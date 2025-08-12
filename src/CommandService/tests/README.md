# CommandService Tests

This folder contains the unit and integration tests for the CommandService.

## Projects

- `CommandService.UnitTests` – verifies application logic by isolating dependencies with mocks.
- `CommandService.IntegrationTests` – spins up the full API using a custom `WebApplicationFactory` that seeds an in-memory database.

## Running tests

Run all tests for the service from the solution file:

```bash
dotnet test CommandService.sln
```

The command executes both unit and integration tests.
