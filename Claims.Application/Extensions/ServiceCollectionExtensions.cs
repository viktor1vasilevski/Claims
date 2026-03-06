using Claims.Application.Channels;
using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Application.Strategies;
using Claims.Application.Validations.Claims;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddScoped<ICoversService, CoversService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IPremiumCalculator, PremiumCalculator>();

        services.AddValidatorsFromAssemblyContaining<CreateClaimRequestValidator>(ServiceLifetime.Transient);

        services.AddSingleton<AuditChannel>();

        services.AddSingleton<IPremiumRateStrategy, YachtRateStrategy>();
        services.AddSingleton<IPremiumRateStrategy, TankerRateStrategy>();
        services.AddSingleton<IPremiumRateStrategy, PassengerShipRateStrategy>();
        services.AddSingleton<IPremiumRateStrategy, ContainerShipRateStrategy>();
        services.AddSingleton<IPremiumRateStrategy, BulkCarrierRateStrategy>();
        services.AddSingleton<IPremiumCalculator, PremiumCalculator>();

        return services;
    }
}
