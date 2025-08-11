using Microsoft.Extensions.DependencyInjection;
using PlatformService.Application.AppServices.Interfaces;

namespace PlatformService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddScoped<IPlatformService, PlatformService.Application.Services.PlatformService>();
        return services;
    }
}
