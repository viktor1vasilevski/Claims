using Claims.Application.Channels;
using Claims.Application.Interfaces;

namespace Claims.Application.Services;

public class AuditService(AuditChannel _auditChannel) : IAuditService
{
    public async Task AuditClaimAsync(string id, string httpRequestType)
    {
        await _auditChannel.Writer.WriteAsync(new AuditMessage(id, httpRequestType, "Claim"));
    }

    public async Task AuditCoverAsync(string id, string httpRequestType)
    {
        await _auditChannel.Writer.WriteAsync(new AuditMessage(id, httpRequestType, "Cover"));
    }
}
