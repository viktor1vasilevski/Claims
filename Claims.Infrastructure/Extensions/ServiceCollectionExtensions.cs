using Claims.Domain.Interfaces;
using Claims.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IClaimsRepository, ClaimsRepository>();
        //services.AddScoped<ICoversRepository, CoversRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();

        return services;
    }
}