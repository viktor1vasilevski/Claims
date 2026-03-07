using Claims.Domain.Models;

namespace Claims.Domain.Interfaces;

public interface IAuditRepository
{
    Task AddClaimAuditAsync(ClaimAudit claimAudit, CancellationToken cancellationToken = default);
    Task AddCoverAuditAsync(CoverAudit coverAudit, CancellationToken cancellationToken = default);
}
