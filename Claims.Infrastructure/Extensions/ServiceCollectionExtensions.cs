using Claims.Infrastructure.BackgroundServices;
using Claims.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Claims.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IClaimsRepository, ClaimsRepository>();
        services.AddScoped<ICoversRepository, CoversRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();

        services.AddSingleton(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<AuditBackgroundService>>();
            return new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        logger.LogWarning(args.Outcome.Exception,
                            "Retrying MongoDB write. Attempt {Attempt}", args.AttemptNumber + 1);
                        return default;
                    }
                })
                .Build();
        });

        services.AddHostedService<AuditBackgroundService>();

        return services;
    }
}