using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Application.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Application.Extensions;

public static class PremiumStrategyCollectionExtensions
{
    public static IServiceCollection AddPremiumStrategies(this IServiceCollection services)
    {
        services.AddSingleton<IPremiumRateStrategy, YachtRateStrategy>();
        services.AddSingleton<IPremiumRateStrategy, TankerRateStrategy>();
        services.AddSingleton<IPremiumRateStrategy, PassengerShipRateStrategy>();
        services.AddSingleton<IPremiumRateStrategy, ContainerShipRateStrategy>();
        services.AddSingleton<IPremiumRateStrategy, BulkCarrierRateStrategy>();
        services.AddSingleton<IPremiumCalculator, PremiumCalculator>();

        return services;
    }
}
