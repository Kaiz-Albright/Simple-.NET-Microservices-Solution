using Company.Platform.Application.Repos;
using Company.Platform.Application.SyncDataServices.Http;
using Company.Platform.Infrastructure.Data;
using Company.Platform.Infrastructure.Data.Repos;
using Company.Platform.Infrastructure.SyncDataServices.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Company.Platform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            services.AddDbContext<AppDbContext>(o =>
                o.UseSqlServer(config.GetConnectionString("PlatformsConnection")));
        }
        else
        {
            services.AddDbContext<AppDbContext>(o =>
                o.UseInMemoryDatabase("InMem"));
        }

        services.AddScoped<IPlatformRepo, PlatformRepo>();
        services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

        return services;
    }
}
