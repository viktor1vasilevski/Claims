using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Claims.Infrastructure.BackgroundServices;

public class AuditBackgroundService(IAuditMessageReceiver messageReceiver, IServiceScopeFactory scopeFactory,
    IAuditMessageProcessor processor, ILogger<AuditBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in messageReceiver.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var auditRepository = scope.ServiceProvider.GetRequiredService<IAuditRepository>();
                await processor.ProcessAsync(auditRepository, message, CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process audit message for {EntityType} {Id}",
                    message.EntityType, message.Id);
            }
        }
    }
}
