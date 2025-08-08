# Simple .NET Microservices Solution

This repository demonstrates a lightweight microservices architecture built with .NET 9.0. It contains two small services that communicate over HTTP.

## Services

- **PlatformService** – exposes a REST API for managing software platforms. When a platform is created it forwards the data to the CommandsService.
- **CommandsService** – accepts platform data sent from the PlatformService and provides a basic endpoint for testing inbound requests.

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- A tool such as `curl` or an API client to issue HTTP requests

## Setup

Restore dependencies and build the solution:

```bash
dotnet restore
dotnet build SimpleDotnetMicroservices.sln
```

## Running the services

Run each service in separate terminals:

```bash
# Terminal 1 – PlatformService
dotnet run --project PlatformService

# Terminal 2 – CommandsService
dotnet run --project CommandsService
```

By default PlatformService listens on `http://localhost:5028` and CommandsService on `http://localhost:5066`.

## Testing

Use `curl` to exercise the APIs:

```bash
# Retrieve platforms
curl http://localhost:5028/api/platforms

# Create a new platform
curl -X POST http://localhost:5028/api/platforms \
  -H "Content-Type: application/json" \
  -d '{"name":"Demo","publisher":"Acme","cost":"Free"}'

# Test the CommandsService endpoint
curl -X POST http://localhost:5066/api/c/platforms
```

Alternatively, use the `.http` files in each project if your editor supports them.

## Visual test execution and reports

you can generate a visual HTML test report and a clickable coverage report.

One-time setup (per machine):

```powershell
# from repo root (Windows PowerShell)
powershell.exe -NoProfile -ExecutionPolicy Bypass -File scripts\Test-Visual.ps1 -NoOpen
```

That will ensure required local tools are installed.

Regular usage:

```powershell
# from repo root (opens reports when done)
powershell.exe -NoProfile -ExecutionPolicy Bypass -File scripts\Test-Visual.ps1
```

This will:
- run `dotnet test` with TRX output and Cobertura coverage
- generate TestResults/index.html (test summary). If an external TRX→HTML tool is unavailable, a built‑in fallback renderer is used
- generate CoverageReport/index.html (coverage by file/line)
- open available reports in your browser (omit `-NoOpen` to skip opening)

What’s included in the HTML test report:
- Filter by text (test name/details) and by outcome (All/Passed/Failed/Skipped)
- Sort by Test, Outcome, or Duration via clickable headers
- Live visible row count

Ignored artifacts in git:
- TestResults/, CoverageReport/, and *.trx files are ignored by .gitignore to keep the repo clean

Troubleshooting:
- If reports do not open automatically, open them manually:
  - Test report: TestResults/index.html
  - Coverage: CoverageReport/index.html
- If you use PowerShell 7 (pwsh), you can run: `pwsh scripts/Test-Visual.ps1`
- If ReportGenerator isn’t available, coverage HTML will be skipped; re-run after `dotnet tool install dotnet-reportgenerator-globaltool`

