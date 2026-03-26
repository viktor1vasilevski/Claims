using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace Claims.Infrastructure.BackgroundServices;

public class AuditBackgroundService(IAuditMessageReceiver messageReceiver, IServiceScopeFactory scopeFactory,
    IAuditMessageProcessor processor, ILogger<AuditBackgroundService> logger,
    ResiliencePipeline retryPipeline) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var envelope in messageReceiver.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var auditRepository = scope.ServiceProvider.GetRequiredService<IAuditRepository>();

                await retryPipeline.ExecuteAsync(
                    async ct => await processor.ProcessAsync(auditRepository, envelope.Message, ct),
                    stoppingToken);

                await envelope.AcknowledgeAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process audit message for {EntityType} {Id} after all retries",
                    envelope.Message.EntityType, envelope.Message.Id);
            }
        }
    }
}
