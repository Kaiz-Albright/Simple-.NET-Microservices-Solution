using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Company.Platform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<Services.Interfaces.IPlatformService, Services.PlatformService>();
        return services;
    }
}
