using Claims.Application.Channels;

namespace Claims.Application.Services;

public class AuditService(IAuditMessageSender messageSender) : IAuditService
{
    public Task AuditClaimAsync(string id, HttpRequestType httpRequestType)
        => messageSender.SendAsync(new AuditMessage(id, httpRequestType, AuditEntityType.Claim));

    public Task AuditCoverAsync(string id, HttpRequestType httpRequestType)
        => messageSender.SendAsync(new AuditMessage(id, httpRequestType, AuditEntityType.Cover));
}
