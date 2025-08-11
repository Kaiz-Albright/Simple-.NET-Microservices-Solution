using Microsoft.Extensions.DependencyInjection;
using PlatformService.Application.Services;

namespace PlatformService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddTransient<IPlatformService, PlatformService.Application.Services.PlatformService>();
        return services;
    }
}
