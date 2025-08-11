using CommandService.Application.AppServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CommandService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddTransient<ICommandService, Services.CommandService>();
        services.AddTransient<IPlatformService, Services.PlatformService>();
        return services;
    }
}
