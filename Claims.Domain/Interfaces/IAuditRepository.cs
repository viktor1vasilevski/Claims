using Claims.Domain.Models;

namespace Claims.Domain.Interfaces;

public interface IAuditRepository
{
    Task AddClaimAuditAsync(ClaimAudit claimAudit);
    Task AddCoverAuditAsync(CoverAudit coverAudit);
}
