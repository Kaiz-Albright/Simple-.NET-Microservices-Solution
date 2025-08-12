using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CommandsService;
using CommandService.Api.Consumers;
using CommandService.Infrastructure.Data;
using CommandService.Domain.Entities;
using CommandService.Application.Contracts.Services;
using RabbitMQ.Client;

namespace CommandService.IntegrationTests;

/// <summary>
/// Custom <see cref="WebApplicationFactory{TEntryPoint}"/> for integration tests.
/// It swaps the real database for an in-memory provider with seeded data.
/// </summary>
public class CommandServiceApiFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Configures the test host with in-memory storage and seeded data.
    /// </summary>
    /// <param name="builder">The web host builder used for the test server.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CommandDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var databaseName = Guid.NewGuid().ToString();
            services.AddDbContext<CommandDbContext>(options =>
                options.UseInMemoryDatabase(databaseName));

            var hostedDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IHostedService) &&
                d.ImplementationType == typeof(PlatformCreatedBackgroundService));
            if (hostedDescriptor != null)
            {
                services.Remove(hostedDescriptor);
            }

            var busDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMessageBusSubscriber));
            if (busDescriptor != null)
            {
                services.Remove(busDescriptor);
            }
            services.AddSingleton<IMessageBusSubscriber, FakeMessageBusSubscriber>();

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CommandDbContext>();
            db.Database.EnsureCreated();

            var platform = new Platform { Id = 1, ExternalID = 100, Name = "Seed" };
            db.Platforms.Add(platform);
            db.Commands.AddRange(
                new Command { HowTo = "HT1", CommandLine = "CL1", PlatformId = platform.Id },
                new Command { HowTo = "HT2", CommandLine = "CL2", PlatformId = platform.Id }
            );
            db.SaveChanges();
        });
    }

    private class FakeMessageBusSubscriber : IMessageBusSubscriber
    {
        public Task SubscribeAsync() => Task.CompletedTask;
        public IChannel GetChannel() => null!;
        public string GetQueueName() => string.Empty;
    }
}
