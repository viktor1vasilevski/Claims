using Claims.Application.Interfaces;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;

namespace Claims.Application.Services;

public class AuditService(IAuditRepository _auditRepository) : IAuditService
{
    public async Task AuditClaimAsync(string id, string httpRequestType)
    {
        var claimAudit = new ClaimAudit
        {
            ClaimId = id,
            Created = DateTime.UtcNow,
            HttpRequestType = httpRequestType
        };
        await _auditRepository.AddClaimAuditAsync(claimAudit);
    }

    public async Task AuditCoverAsync(string id, string httpRequestType)
    {
        var coverAudit = new CoverAudit
        {
            CoverId = id,
            Created = DateTime.UtcNow,
            HttpRequestType = httpRequestType
        };
        await _auditRepository.AddCoverAuditAsync(coverAudit);
    }
}
