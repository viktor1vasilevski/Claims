using Claims.Application.Interfaces;
using Claims.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddScoped<ICoversService, CoversService>();
        services.AddScoped<IAuditService, AuditService>();

        services.AddSingleton<IAuditMessageProcessor, AuditMessageProcessor>();

        services.AddPremiumStrategies();

        return services;
    }
}
