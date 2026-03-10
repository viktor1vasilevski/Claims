using Claims.Application.Channels;
using Claims.Application.Interfaces;
using Claims.Domain.Enums;

namespace Claims.Application.Services;

public class AuditService(AuditChannel auditChannel) : IAuditService
{
    public async Task AuditClaimAsync(string id, HttpRequestType httpRequestType)
    {
        await auditChannel.Writer.WriteAsync(new AuditMessage(id, httpRequestType, AuditEntityType.Claim));
    }

    public async Task AuditCoverAsync(string id, HttpRequestType httpRequestType)
    {
        await auditChannel.Writer.WriteAsync(new AuditMessage(id, httpRequestType, AuditEntityType.Cover));
    }
}
