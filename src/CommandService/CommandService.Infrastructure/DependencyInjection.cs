using CommandService.Application.Repositories;
using CommandService.Infrastructure.Data;
using CommandService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CommandService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<CommandDbContext>(o =>
            o.UseInMemoryDatabase("CommandServiceDb"));

        services.AddScoped<ICommandRepo, CommandRepo>();
        services.AddScoped<IPlatformRepo, PlatformRepo>();

        return services;
    }
}
