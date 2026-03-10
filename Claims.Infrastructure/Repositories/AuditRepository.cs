using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using Claims.Infrastructure.Context;

namespace Claims.Infrastructure.Repositories;

public class AuditRepository(AuditContext auditContext) : IAuditRepository
{
    public async Task AddClaimAuditAsync(ClaimAudit claimAudit, CancellationToken cancellationToken = default)
    {
        auditContext.ClaimAudits.Add(claimAudit);
        await auditContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddCoverAuditAsync(CoverAudit coverAudit, CancellationToken cancellationToken = default)
    {
        auditContext.CoverAudits.Add(coverAudit);
        await auditContext.SaveChangesAsync(cancellationToken);
    }
}
