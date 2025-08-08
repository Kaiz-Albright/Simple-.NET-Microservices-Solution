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

