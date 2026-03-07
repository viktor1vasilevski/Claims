using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using Claims.Infrastructure.Context;

namespace Claims.Infrastructure.Repositories;

public class AuditRepository(AuditContext _auditContext) : IAuditRepository
{
    public async Task AddClaimAuditAsync(ClaimAudit claimAudit, CancellationToken cancellationToken = default)
    {
        _auditContext.ClaimAudits.Add(claimAudit);
        await _auditContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddCoverAuditAsync(CoverAudit coverAudit, CancellationToken cancellationToken = default)
    {
        _auditContext.CoverAudits.Add(coverAudit);
        await _auditContext.SaveChangesAsync(cancellationToken);
    }
}