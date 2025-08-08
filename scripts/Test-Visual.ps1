param(
  [switch]$NoOpen
)

$ErrorActionPreference = 'Stop'

# Resolve repository root (script is in scripts/)
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

Push-Location $repoRoot
try {
  Write-Host "Running tests with TRX and coverage..."
  dotnet test $repoRoot --settings (Join-Path $repoRoot 'tests.runsettings') --logger "trx;LogFileName=test.trx" --results-directory $resultsDir --collect "XPlat Code Coverage"

  $trx = Join-Path $resultsDir 'test.trx'
  if (-not (Test-Path $trx)) { throw "TRX file not found: $trx" }

  function Convert-TrxToHtmlSimple {
    param([string]$TrxPath,[string]$HtmlPath,[string]$Title)
    [xml]$xml = Get-Content $TrxPath -Raw
    $nsm = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
    $nsm.AddNamespace('t', $xml.DocumentElement.NamespaceURI)
    $results = $xml.SelectNodes('//t:UnitTestResult', $nsm)
    $summary = $xml.SelectSingleNode('//t:Counters', $nsm)
    $times = $xml.SelectSingleNode('//t:Times', $nsm)

    function HtmlEscape([string]$s) {
      if ($null -eq $s) { return '' }
      $s = $s -replace '&','&amp;' -replace '<','&lt;' -replace '>','&gt;'
      $s = $s -replace '"','&quot;' -replace "'",'&#39;'
      return $s
    }

    if ($null -eq $summary) {
      # Derive counts from results if summary missing
      $total = $results.Count
      $passed = ($results | Where-Object { $_.outcome -eq 'Passed' }).Count
      $failed = ($results | Where-Object { $_.outcome -eq 'Failed' }).Count
      $skipped = ($results | Where-Object { $_.outcome -ne 'Passed' -and $_.outcome -ne 'Failed' }).Count
    } else {
      $total = [int]($summary.total)
      $passed = [int]($summary.passed)
      $failed = [int]($summary.failed)
      $skipped = [int]($summary.notExecuted)
    }
    $started = if ($times) { $times.start } else { '' }
    $finished = if ($times) { $times.finish } else { '' }

    $rows = @()
    foreach ($r in $results) {
      $name = HtmlEscape $r.testName
      $outcome = HtmlEscape $r.outcome
      $duration = HtmlEscape $r.duration
      $errMsg = ''
      $stack = ''
      $out = $r.SelectSingleNode('t:Output/t:ErrorInfo/t:Message', $nsm)
      if ($out) { $errMsg = HtmlEscape $out.InnerText }
      $st = $r.SelectSingleNode('t:Output/t:ErrorInfo/t:StackTrace', $nsm)
      if ($st) { $stack = HtmlEscape $st.InnerText }
      $rows += "<tr class='outcome-$outcome'><td>$name</td><td>$outcome</td><td>$duration</td><td><pre>$errMsg`n$stack</pre></td></tr>"
    }

    $titleEsc = HtmlEscape $Title
    $styleLines = @(
      'body{font-family:Segoe UI,Arial,Helvetica,sans-serif;margin:16px}',
      '.cards{display:flex;gap:12px;margin:12px 0}',
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

    $htmlLines = @()
    $htmlLines += '<!DOCTYPE html>'
    $htmlLines += '<html>'
    $htmlLines += '<head>'
    $htmlLines += "<meta charset='utf-8'/>"
    $htmlLines += "<title>$titleEsc</title>"
    $htmlLines += '<style>'
    $htmlLines += $styleLines
    $htmlLines += '</style>'
    $htmlLines += '</head>'
    $htmlLines += '<body>'
    $htmlLines += "<h1>$titleEsc</h1>"
    $htmlLines += "<div>Started: $started&nbsp; Finished: $finished</div>"
    $htmlLines += "<div class='cards'>"
    $htmlLines += "  <div class='card'>Total: <b>$total</b></div>"
    $htmlLines += "  <div class='card ok'>Passed: <b>$passed</b></div>"
    $htmlLines += "  <div class='card fail'>Failed: <b>$failed</b></div>"
    $htmlLines += "  <div class='card skip'>Skipped: <b>$skipped</b></div>"
    $htmlLines += '</div>'

    # Controls for filtering & sorting
    $htmlLines += "<div style='margin:12px 0; display:flex; gap:12px; align-items:center'>"
    $htmlLines += "  <label for='filterText'>Filter:</label> <input id='filterText' type='text' placeholder='Search test name or details' style='flex:1; padding:6px 8px' />"
    $htmlLines += "  <label for='filterOutcome'>Outcome:</label> <select id='filterOutcome' style='padding:6px 8px'><option value=''>All</option><option>Passed</option><option>Failed</option><option value='NotExecuted'>Skipped</option></select>"
    $htmlLines += "  <button id='clearFilters' style='padding:6px 10px'>Clear</button>"
    $htmlLines += "  <span id='rowCount' style='margin-left:auto; color:#666'></span>"
    $htmlLines += "</div>"

    $htmlLines += "<table id='resultsTable'>"
    $htmlLines += '  <thead><tr><th class="sortable" data-col="0">Test</th><th class="sortable" data-col="1">Outcome</th><th class="sortable" data-col="2">Duration</th><th>Details</th></tr></thead>'
    $htmlLines += '  <tbody>'
    $htmlLines += ($rows -join "`n")
    $htmlLines += '  </tbody>'
    $htmlLines += '</table>'

    # Client-side JS for filtering and sorting
    $htmlLines += '<script>'
    $htmlLines += "(function(){"
    $htmlLines += "var txt=document.getElementById('filterText');"
    $htmlLines += "var sel=document.getElementById('filterOutcome');"
    $htmlLines += "var clr=document.getElementById('clearFilters');"
    $htmlLines += "var table=document.getElementById('resultsTable');"
    $htmlLines += "var tbody=table.querySelector('tbody');"
    $htmlLines += "var count=document.getElementById('rowCount');"
    $htmlLines += "function norm(s){return (s||'').toLowerCase();}"
    $htmlLines += "function filter(){var q=norm(txt.value);var oc=sel.value;var shown=0;Array.from(tbody.rows).forEach(function(r){var name=r.cells[0].textContent;var outcome=r.cells[1].textContent;var details=r.cells[3].textContent;var ok=true;if(q){ok=norm(name).indexOf(q)>-1||norm(details).indexOf(q)>-1;}if(ok&&oc){ok=outcome===oc;}r.style.display=ok?'':'none';if(ok)shown++;});count.textContent=shown+' / '+tbody.rows.length+' visible';}"
    $htmlLines += "txt.addEventListener('input',filter);sel.addEventListener('change',filter);clr.addEventListener('click',function(){txt.value='';sel.value='';filter();});"
    $htmlLines += "function cmp(a,b,dir){if(a===b)return 0;if(a==null)return -1*dir;if(b==null)return 1*dir;if(!isNaN(a)&&!isNaN(b)){return (parseFloat(a)-parseFloat(b))*dir;}return a.localeCompare(b)*dir;}"
    $htmlLines += "function sort(col){var dir=1;var th=table.tHead.rows[0].cells[col];var cur=th.getAttribute('data-dir');if(cur==='asc'){dir=-1;th.setAttribute('data-dir','desc');}else{dir=1;th.setAttribute('data-dir','asc');}Array.from(table.tHead.rows[0].cells).forEach(function(x){if(x!==th)x.removeAttribute('data-dir');});var rows=Array.from(tbody.rows);rows.sort(function(r1,r2){var a=r1.cells[col].textContent.trim();var b=r2.cells[col].textContent.trim();if(col===1){var map={'Passed':2,'Failed':1,'NotExecuted':0};a=(map[a]!==undefined?map[a]:-1);b=(map[b]!==undefined?map[b]:-1);}return cmp(a,b,dir);});rows.forEach(function(r){tbody.appendChild(r);});}"
    $htmlLines += "Array.from(table.tHead.rows[0].cells).forEach(function(th){if(th.classList.contains('sortable')){th.style.cursor='pointer';th.title='Click to sort';th.addEventListener('click',function(){sort(parseInt(th.getAttribute('data-col')));});}});"
    $htmlLines += "filter();"
    $htmlLines += "})();"
    $htmlLines += '</script>'

    $htmlLines += '</body>'
    $htmlLines += '</html>'

    $dir = Split-Path $HtmlPath -Parent
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllLines($HtmlPath, $htmlLines, $utf8NoBom)
    $fi = Get-Item $HtmlPath -ErrorAction SilentlyContinue
    if ($fi) { Write-Host "Wrote HTML report: $($fi.FullName) ($([int]$fi.Length) bytes)" }
  }

  if ($hasTrx2Html) {
    Write-Host "Generating HTML test report (external tool)..."
    dotnet tool run trx2html $trx --title "PlatformMicros Tests" --output (Join-Path $resultsDir 'index.html')
  } else {
    Write-Host "Generating HTML test report (built-in fallback)..."
    Convert-TrxToHtmlSimple -TrxPath $trx -HtmlPath (Join-Path $resultsDir 'index.html') -Title 'PlatformMicros Tests'
  }

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
