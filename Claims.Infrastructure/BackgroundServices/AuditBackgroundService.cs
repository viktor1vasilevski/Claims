using Claims.Application.Channels;
using Claims.Domain.Enums;
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
                await ProcessAuditMessageAsync(auditRepository, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process audit message for {EntityType} {Id}",
                    message.EntityType, message.Id);
            }
        }
    }

    private static Task ProcessAuditMessageAsync(IAuditRepository auditRepository, AuditMessage message)
    => message.EntityType switch
    {
        AuditEntityType.Claim => auditRepository.AddClaimAuditAsync(new ClaimAudit
        {
            ClaimId = message.Id,
            Created = DateTime.UtcNow,
            HttpRequestType = message.HttpRequestType
        }),
        AuditEntityType.Cover => auditRepository.AddCoverAuditAsync(new CoverAudit
        {
            CoverId = message.Id,
            Created = DateTime.UtcNow,
            HttpRequestType = message.HttpRequestType
        }),
        _ => throw new ArgumentOutOfRangeException(nameof(message.EntityType),
            $"Unhandled audit entity type: {message.EntityType}")
    };
}   