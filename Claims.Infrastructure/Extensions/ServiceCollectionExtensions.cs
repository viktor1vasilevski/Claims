using Claims.Infrastructure.BackgroundServices;
using Claims.Infrastructure.Repositories;

namespace Claims.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IClaimsRepository, ClaimsRepository>();
        services.AddScoped<ICoversRepository, CoversRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();

        services.AddHostedService<AuditBackgroundService>();

        return services;
    }
}