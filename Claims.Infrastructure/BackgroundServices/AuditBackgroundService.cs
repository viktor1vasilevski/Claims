using Claims.Application.Channels;
using Claims.Application.Interfaces;
using Claims.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Claims.Infrastructure.BackgroundServices;

public class AuditBackgroundService(AuditChannel _auditChannel, IServiceScopeFactory _scopeFactory,
    IAuditMessageProcessor _processor, ILogger<AuditBackgroundService> _logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _auditChannel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var auditRepository = scope.ServiceProvider.GetRequiredService<IAuditRepository>();
                await _processor.ProcessAsync(auditRepository, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process audit message for {EntityType} {Id}",
                    message.EntityType, message.Id);
            }
        }
    }
}