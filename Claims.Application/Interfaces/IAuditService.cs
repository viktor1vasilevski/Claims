namespace Claims.Application.Interfaces;

public interface IAuditService
{
    Task AuditClaimAsync(string id, string httpRequestType);
    Task AuditCoverAsync(string id, string httpRequestType);
}
