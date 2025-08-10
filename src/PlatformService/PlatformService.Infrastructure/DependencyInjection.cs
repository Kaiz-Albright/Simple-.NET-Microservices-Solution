using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlatformService.Infrastructure.Data;
using PlatformService.Infrastructure.Repositories;
using PlatformService.Application.Repositories;
using PlatformService.Infrastructure.SyncDataServices.Http;
using PlatformService.Infrastructure.Interfaces;

namespace PlatformService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
    {
        services.AddDbContext<PlatformDbContext>(options =>
        {
            if (env.IsDevelopment())
            {
                Console.WriteLine("--> Using InMemory Database");
                options.UseInMemoryDatabase("PlatformServiceDb");
            }
            else
            {
                Console.WriteLine("--> Using SQL Server Database");
                options.UseSqlServer(config.GetConnectionString("PlatformService"));
            }
        });

        services.AddScoped<IPlatformRepo, PlatformRepo>();
        services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

        return services;
    }
}
