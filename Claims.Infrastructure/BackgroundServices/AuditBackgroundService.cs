using Claims.Application.Channels;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;
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
                var auditRepository = scope.ServiceProvider.GetRequiredService<IAuditRepository>();

                if (message.EntityType == "Claim")
                    await auditRepository.AddClaimAuditAsync(new ClaimAudit
                    {
                        ClaimId = message.Id,
                        Created = DateTime.UtcNow,
                        HttpRequestType = message.HttpRequestType
                    });
                else if (message.EntityType == "Cover")
                    await auditRepository.AddCoverAuditAsync(new CoverAudit
                    {
                        CoverId = message.Id,
                        Created = DateTime.UtcNow,
                        HttpRequestType = message.HttpRequestType
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process audit message for {EntityType} {Id}",
                    message.EntityType, message.Id);
            }
        }
    }
}   