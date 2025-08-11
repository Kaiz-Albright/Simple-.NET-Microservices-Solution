# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0@sha256:3fcf6f1e809c0553f9feb222369f58749af314af6f063f389cbd2f913b4ad556 AS build
WORKDIR /src

# Copy project files for restore
COPY PlatformService.sln ./
COPY PlatformService.Api/PlatformService.Api.csproj PlatformService.Api/
COPY PlatformService.Application/PlatformService.Application.csproj PlatformService.Application/
COPY PlatformService.Infrastructure/PlatformService.Infrastructure.csproj PlatformService.Infrastructure/
COPY PlatformService.Domain/PlatformService.Domain.csproj PlatformService.Domain/

# Restore only using the project files to leverage Docker layer caching
RUN dotnet restore PlatformService.Api/PlatformService.Api.csproj

# Copy the rest of the source
COPY . .
RUN dotnet restore

# Publish
WORKDIR /src/PlatformService.Api
RUN dotnet publish -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0@sha256:b4bea3a52a0a77317fa93c5bbdb076623f81e3e2f201078d89914da71318b5d8
WORKDIR /App
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "PlatformService.Api.dll"]
