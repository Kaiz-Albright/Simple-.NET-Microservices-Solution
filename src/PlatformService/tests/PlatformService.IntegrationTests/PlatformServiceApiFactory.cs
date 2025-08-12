using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Infrastructure.Data;
using PlatformService.Api;
using PlatformService.Application.Contracts.Services;
using PlatformService.Application.Dtos;
using PlatformService.Domain.Entities;

namespace PlatformService.IntegrationTests;

/// <summary>
/// Custom <see cref="WebApplicationFactory{TEntryPoint}"/> for integration tests.
/// It swaps the real database for an in-memory provider and replaces external
/// service clients with fakes so tests run deterministically.
/// </summary>
public class PlatformServiceApiFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Configures the test host with in-memory storage and seeded data.
    /// </summary>
    /// <param name="builder">The web host builder used for the test server.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PlatformDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var databaseName = Guid.NewGuid().ToString();
            services.AddDbContext<PlatformDbContext>(options =>
                options.UseInMemoryDatabase(databaseName));

            var cmdDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ICommandDataClient));
            if (cmdDescriptor != null)
            {
                services.Remove(cmdDescriptor);
            }
            // Replace outbound HTTP communication with a fake client.
            services.AddSingleton<ICommandDataClient, FakeCommandDataClient>();

            var busDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMessageBusClient));
            if (busDescriptor != null)
            {
                services.Remove(busDescriptor);
            }
            // Replace the message bus with a fake implementation.
            services.AddSingleton<IMessageBusClient, FakeMessageBusClient>();

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
            db.Database.EnsureCreated();
            db.Platforms.AddRange(
                new Platform { Name = "Seed1", Publisher = "Pub1", Cost = "Free" },
                new Platform { Name = "Seed2", Publisher = "Pub2", Cost = "Free" },
                new Platform { Name = "Seed3", Publisher = "Pub3", Cost = "Free" }
            );
            db.SaveChanges();
        });
    }

    /// <summary>
    /// Minimal ICommandDataClient stub used during tests to avoid real HTTP calls.
    /// </summary>
    private class FakeCommandDataClient : ICommandDataClient
    {
        public Task SendPlatformToCommand(PlatformReadDto platform) => Task.CompletedTask;
    }

    /// <summary>
    /// Minimal IMessageBusClient stub used during tests to avoid real message bus traffic.
    /// </summary>
    private class FakeMessageBusClient : IMessageBusClient
    {
        public Task PublishNewPlatform(PlatformPublishedDto platformPublishedDto) => Task.CompletedTask;
    }
}
