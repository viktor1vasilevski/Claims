namespace Claims.Application.Interfaces.Services;

public interface IAuditService
{
    Task AuditClaimAsync(string id, HttpRequestType httpRequestType);
    Task AuditCoverAsync(string id, HttpRequestType httpRequestType);
}
