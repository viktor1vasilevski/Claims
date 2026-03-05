using Claims.Application.Channels;
using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Application.Strategies;
using Claims.Application.Validations.Claims;
using Claims.Domain.Enums;
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

        services.AddSingleton<ICoverRatePolicy, YachtRatePolicy>();
        services.AddSingleton<ICoverRatePolicy, PassengerShipRatePolicy>();
        services.AddSingleton<ICoverRatePolicy, TankerRatePolicy>();
        services.AddSingleton<ICoverRatePolicy>(_ => new DefaultRatePolicy(CoverType.ContainerShip));
        services.AddSingleton<ICoverRatePolicy>(_ => new DefaultRatePolicy(CoverType.BulkCarrier));

        services.AddValidatorsFromAssemblyContaining<CreateClaimRequestValidator>(ServiceLifetime.Transient);

        services.AddSingleton<AuditChannel>();

        return services;
    }
}
