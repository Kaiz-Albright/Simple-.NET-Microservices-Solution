using Microsoft.Extensions.DependencyInjection;

namespace PlatformService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        return services;
    }
}
