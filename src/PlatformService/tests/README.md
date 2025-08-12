# PlatformService Tests

This folder contains the unit and integration tests for the PlatformService.

## Projects

- `PlatformService.UnitTests` – verifies application and repository logic by isolating dependencies with mocks and an in-memory database.
- `PlatformService.IntegrationTests` – spins up the full API using a custom `WebApplicationFactory` that swaps external services for fakes and seeds an in-memory database.

## Running tests

Run all tests for the service from the solution file:

```bash
dotnet test PlatformService.sln
```

The command executes both unit and integration tests.

