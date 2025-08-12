param(
  [switch]$NoOpen
)

$ErrorActionPreference = 'Stop'

# Resolve repository root (script is in testing/scripts/)
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$toolsJson = Join-Path $repoRoot '.config/dotnet-tools.json'

# Ensure tool manifest exists for local tools in repo root
if (-not (Test-Path $toolsJson)) {
  dotnet new tool-manifest --force --output $repoRoot | Out-Null
}

# Install required tools locally if missing (package IDs as keys in dotnet-tools.json)
function Ensure-DotnetTool {
  param(
    [Parameter(Mandatory=$true)][string]$PackageId,
    [Parameter(Mandatory=$true)][string]$DisplayName
  )
  $installed = $false
  if (Test-Path $toolsJson) {
    $json = Get-Content $toolsJson -Raw | ConvertFrom-Json
    $installed = $null -ne $json.tools -and ($json.tools.PSObject.Properties.Name -contains $PackageId)
  }
  if (-not $installed) {
    Write-Host "Installing $DisplayName..."
    try {
      dotnet tool install $PackageId --tool-manifest $toolsJson | Out-Null
    } catch {
      Write-Warning "Failed to install $DisplayName ($PackageId). Will continue without it."
    }
    # Re-check after attempted install
    if (Test-Path $toolsJson) {
      $json = Get-Content $toolsJson -Raw | ConvertFrom-Json
      $installed = $null -ne $json.tools -and ($json.tools.PSObject.Properties.Name -contains $PackageId)
    }
  }
  return $installed
}

# Try multiple package IDs for trx2html (some feeds publish different IDs)
$hasTrx2Html = $false
$trxCandidates = @(
  @{ Id = 'trx2html'; Name = 'TRX to HTML converter' },
  @{ Id = 'dotMorten.Trx2Html'; Name = 'TRX to HTML converter (dotMorten)' }
)
foreach ($pkg in $trxCandidates) {
  if (-not $hasTrx2Html) {
    if (Ensure-DotnetTool -PackageId $pkg.Id -DisplayName $pkg.Name) {
      $hasTrx2Html = $true
    }
  }
}

$hasReportGen = Ensure-DotnetTool -PackageId "dotnet-reportgenerator-globaltool" -DisplayName "ReportGenerator"

$resultsDir = Join-Path $repoRoot 'TestResults'
$coverageDir = Join-Path $repoRoot 'CoverageReport'
New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null
New-Item -ItemType Directory -Force -Path $coverageDir | Out-Null

# Discover all test projects (*.Tests.csproj / *Tests.csproj) under src/
$tests = Get-ChildItem -LiteralPath (Join-Path $repoRoot 'src') -Recurse -File -Include *Tests.csproj,*.Tests.csproj | Sort-Object FullName
if (-not $tests) {
  Write-Warning "No test projects found under src/. Running dotnet test at repo root as fallback."
}

Push-Location $repoRoot
try {
  $runsettings = Join-Path $repoRoot 'testing/tests.runsettings'
  if (-not (Test-Path $runsettings)) {
    Write-Warning "RunSettings not found at '$runsettings'. Coverage may be limited."
  }

  Write-Host "Running tests with TRX and coverage..."
  if ($tests) {
    foreach ($proj in $tests) {
      $name = [System.IO.Path]::GetFileNameWithoutExtension($proj.Name)
      $logName = "$name.trx"
      Write-Host " - $name"
      dotnet test $proj.FullName --nologo --no-build --logger "trx;LogFileName=$logName" --results-directory $resultsDir @(
        if (Test-Path $runsettings) { '--settings'; $runsettings }
      ) --collect "XPlat Code Coverage"
    }
  } else {
    dotnet test $repoRoot --nologo --logger "trx;LogFileName=AllTests.trx" --results-directory $resultsDir @(
      if (Test-Path $runsettings) { '--settings'; $runsettings }
    ) --collect "XPlat Code Coverage"
  }

  # Collect all TRX files produced
  $trxFiles = Get-ChildItem -LiteralPath $resultsDir -Filter *.trx -File | Sort-Object Name
  if (-not $trxFiles) { throw "No TRX files were generated in $resultsDir" }

  function Convert-TrxToHtmlMulti {
    param(
      [Parameter(Mandatory=$true)][System.IO.FileInfo[]]$TrxFiles,
      [Parameter(Mandatory=$true)][string]$HtmlPath,
      [Parameter(Mandatory=$true)][string]$Title
    )

    function HtmlEscape([string]$s) {
      if ($null -eq $s) { return '' }
      $s = $s -replace '&','&amp;' -replace '<','&lt;' -replace '>','&gt;'
      $s = $s -replace '"','&quot;' -replace "'",'&#39;'
      return $s
    }

    $allRows = @()
    $grandTotal = 0; $grandPassed = 0; $grandFailed = 0; $grandSkipped = 0

    foreach ($f in $TrxFiles) {
      [xml]$xml = Get-Content $f.FullName -Raw
      $ns = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
      $ns.AddNamespace('t', $xml.DocumentElement.NamespaceURI)
      $results = $xml.SelectNodes('//t:UnitTestResult', $ns)
      $summary = $xml.SelectSingleNode('//t:Counters', $ns)
      $suiteName = [System.IO.Path]::GetFileNameWithoutExtension($f.Name)

      $total = $results.Count
      $passed = ($results | Where-Object { $_.outcome -eq 'Passed' }).Count
      $failed = ($results | Where-Object { $_.outcome -eq 'Failed' }).Count
      $skipped = ($results | Where-Object { $_.outcome -ne 'Passed' -and $_.outcome -ne 'Failed' }).Count
      if ($summary) {
        $total = [int]$summary.total; $passed = [int]$summary.passed; $failed = [int]$summary.failed; $skipped = [int]$summary.notExecuted
      }
      $grandTotal += $total; $grandPassed += $passed; $grandFailed += $failed; $grandSkipped += $skipped

      foreach ($r in $results) {
        $name = HtmlEscape $r.testName
        $outcome = HtmlEscape $r.outcome
        $duration = HtmlEscape $r.duration
        $errMsg = ''
        $stack = ''
        $out = $r.SelectSingleNode('t:Output/t:ErrorInfo/t:Message', $ns)
        if ($out) { $errMsg = HtmlEscape $out.InnerText }
        $st = $r.SelectSingleNode('t:Output/t:ErrorInfo/t:StackTrace', $ns)
        if ($st) { $stack = HtmlEscape $st.InnerText }
        $allRows += "<tr class='outcome-$outcome'><td>" + HtmlEscape($suiteName) + "</td><td>$name</td><td>$outcome</td><td>$duration</td><td><pre>$errMsg`n$stack</pre></td></tr>"
      }
    }

    $titleEsc = HtmlEscape $Title
    $style = @(
      'body{font-family:Segoe UI,Arial,Helvetica,sans-serif;margin:16px}',
      '.cards{display:flex;gap:12px;margin:12px 0;flex-wrap:wrap}',
      '.card{padding:10px 12px;border-radius:8px;background:#f5f5f5;border:1px solid #e0e0e0}',
      '.ok{background:#e8f5e9;border-color:#c8e6c9}',
      '.fail{background:#ffebee;border-color:#ffcdd2}',
      '.skip{background:#fff8e1;border-color:#ffecb3}',
      'table{border-collapse:collapse;width:100%}',
      'th,td{border:1px solid #ddd;padding:8px;vertical-align:top}',
      'th{background:#fafafa;text-align:left}',
      'tr.outcome-Passed{background:#fafdf9}',
      'tr.outcome-Failed{background:#fff8f8}',
      'tr.outcome-NotExecuted{background:#fffdf5}',
      'pre{white-space:pre-wrap;margin:0}'
    ) -join "`n"

    $html = @()
    $html += '<!DOCTYPE html>'
    $html += '<html>'
    $html += '<head>'
    $html += "<meta charset='utf-8'/>"
    $html += "<title>$titleEsc</title>"
    $html += '<style>'
    $html += $style
    $html += '</style>'
    $html += '</head>'
    $html += '<body>'
    $html += "<h1>$titleEsc</h1>"
    $html += "<div class='cards'>"
    $html += "  <div class='card'>Total: <b>$grandTotal</b></div>"
    $html += "  <div class='card ok'>Passed: <b>$grandPassed</b></div>"
    $html += "  <div class='card fail'>Failed: <b>$grandFailed</b></div>"
    $html += "  <div class='card skip'>Skipped: <b>$grandSkipped</b></div>"
    $html += '</div>'

    $html += "<div style='margin:12px 0; display:flex; gap:12px; align-items:center; flex-wrap:wrap'>"
    $html += "  <label for='filterText'>Filter:</label> <input id='filterText' type='text' placeholder='Search test name or details' style='flex:1; min-width:240px; padding:6px 8px' />"
    $html += "  <label for='filterOutcome'>Outcome:</label> <select id='filterOutcome' style='padding:6px 8px'><option value=''>All</option><option>Passed</option><option>Failed</option><option value='NotExecuted'>Skipped</option></select>"
    $html += "  <label for='filterSuite'>Suite:</label> <input id='filterSuite' type='text' placeholder='Project/Test suite' style='min-width:200px; padding:6px 8px' />"
    $html += "  <button id='clearFilters' style='padding:6px 10px'>Clear</button>"
    $html += "  <span id='rowCount' style='margin-left:auto; color:#666'></span>"
    $html += '</div>'

    $html += "<table id='resultsTable'>"
    $html += '  <thead><tr><th class="sortable" data-col="0">Suite</th><th class="sortable" data-col="1">Test</th><th class="sortable" data-col="2">Outcome</th><th class="sortable" data-col="3">Duration</th><th>Details</th></tr></thead>'
    $html += '  <tbody>'
    $html += ($allRows -join "`n")
    $html += '  </tbody>'
    $html += '</table>'

    $html += '<script>'
    $html += "(function(){"
    $html += "var txt=document.getElementById('filterText');var sel=document.getElementById('filterOutcome');var suite=document.getElementById('filterSuite');var clr=document.getElementById('clearFilters');var table=document.getElementById('resultsTable');var tbody=table.querySelector('tbody');var count=document.getElementById('rowCount');function norm(s){return (s||'').toLowerCase();}function filter(){var q=norm(txt.value);var oc=sel.value;var sq=norm(suite.value);var shown=0;Array.from(tbody.rows).forEach(function(r){var s=r.cells[0].textContent;var name=r.cells[1].textContent;var outcome=r.cells[2].textContent;var details=r.cells[4].textContent;var ok=true;if(q){ok=norm(name).indexOf(q)>-1||norm(details).indexOf(q)>-1;}if(ok&&oc){ok=outcome===oc;}if(ok&&sq){ok=norm(s).indexOf(sq)>-1;}r.style.display=ok?'':'none';if(ok)shown++;});count.textContent=shown+' / '+tbody.rows.length+' visible';}txt.addEventListener('input',filter);sel.addEventListener('change',filter);suite.addEventListener('input',filter);clr.addEventListener('click',function(){txt.value='';sel.value='';suite.value='';filter();});function cmp(a,b,dir){if(a===b)return 0;if(a==null)return -1*dir;if(b==null)return 1*dir;if(!isNaN(a)&&!isNaN(b)){return (parseFloat(a)-parseFloat(b))*dir;}return a.localeCompare(b)*dir;}function sort(col){var dir=1;var th=table.tHead.rows[0].cells[col];var cur=th.getAttribute('data-dir');if(cur==='asc'){dir=-1;th.setAttribute('data-dir','desc');}else{dir=1;th.setAttribute('data-dir','asc');}Array.from(table.tHead.rows[0].cells).forEach(function(x){if(x!==th)x.removeAttribute('data-dir');});var rows=Array.from(tbody.rows);rows.sort(function(r1,r2){var a=r1.cells[col].textContent.trim();var b=r2.cells[col].textContent.trim();if(col===2){var map={'Passed':2,'Failed':1,'NotExecuted':0};a=(map[a]!==undefined?map[a]:-1);b=(map[b]!==undefined?map[b]:-1);}return cmp(a,b,dir);});rows.forEach(function(r){tbody.appendChild(r);});}Array.from(table.tHead.rows[0].cells).forEach(function(th){if(th.classList.contains('sortable')){th.style.cursor='pointer';th.title='Click to sort';th.addEventListener('click',function(){sort(parseInt(th.getAttribute('data-col')));});}});filter();" 
    $html += "})();"
    $html += '</script>'

    $html += '</body>'
    $html += '</html>'

    $dir = Split-Path $HtmlPath -Parent
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllLines($HtmlPath, $html, $utf8NoBom)
    $fi = Get-Item $HtmlPath -ErrorAction SilentlyContinue
    if ($fi) { Write-Host "Wrote HTML report: $($fi.FullName) ($([int]$fi.Length) bytes)" }
  }

  # Prefer our multi-TRX aggregator; if external tool available but cannot merge, we still have a full report
  Write-Host "Generating HTML test report..."
  Convert-TrxToHtmlMulti -TrxFiles $trxFiles -HtmlPath (Join-Path $resultsDir 'index.html') -Title 'PlatformMicros Tests'

  if ($hasReportGen) {
    Write-Host "Generating HTML coverage report..."
    dotnet tool run reportgenerator -reports:(Join-Path $resultsDir "**\coverage.cobertura.xml") -targetdir:$coverageDir -reporttypes:Html
  } else {
    Write-Warning "Skipping coverage HTML (ReportGenerator not available)."
  }
}
finally {
  Pop-Location
}

if (-not $NoOpen) {
  $testIndex = Join-Path $resultsDir 'index.html'
  $covIndexHtml = Join-Path $coverageDir 'index.html'
  if (Test-Path $testIndex) { Start-Process $testIndex }
  if (Test-Path $covIndexHtml) { Start-Process $covIndexHtml }
}

Write-Host "Done. Test report (if generated): $resultsDir/index.html  Coverage: $coverageDir/index.html"
