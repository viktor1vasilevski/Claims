using Claims.Application.Channels;
using Claims.Domain.Models;
using Claims.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Claims.Infrastructure.BackgroundServices;

public class AuditBackgroundService(
    AuditChannel _auditChannel,
    IServiceScopeFactory _scopeFactory,
    ILogger<AuditBackgroundService> _logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _auditChannel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var auditContext = scope.ServiceProvider.GetRequiredService<AuditContext>();

                if (message.EntityType == "Claim")
                {
                    auditContext.ClaimAudits.Add(new ClaimAudit
                    {
                        ClaimId = message.Id,
                        Created = DateTime.UtcNow,
                        HttpRequestType = message.HttpRequestType
                    });
                }
                else if (message.EntityType == "Cover")
                {
                    auditContext.CoverAudits.Add(new CoverAudit
                    {
                        CoverId = message.Id,
                        Created = DateTime.UtcNow,
                        HttpRequestType = message.HttpRequestType
                    });
                }

                await auditContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process audit message for {EntityType} {Id}",
                    message.EntityType, message.Id);
            }
        }
    }
}