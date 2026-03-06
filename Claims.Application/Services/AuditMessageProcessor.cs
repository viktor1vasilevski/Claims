using Claims.Application.Channels;
using Claims.Application.Interfaces;
using Claims.Domain.Enums;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;

namespace Claims.Application.Services;

public class AuditMessageProcessor : IAuditMessageProcessor
{
    public Task ProcessAsync(IAuditRepository auditRepository, AuditMessage message)
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
