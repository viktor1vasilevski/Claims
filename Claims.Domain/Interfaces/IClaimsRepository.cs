using Claims.Domain.Models;

namespace Claims.Domain.Interfaces;

public interface IClaimsRepository
{
    Task<IReadOnlyList<Claim>> GetClaimsAsync(CancellationToken cancellationToken = default);
    Task<Claim?> GetClaimByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateClaimAsync(Claim claim, CancellationToken cancellationToken = default);
    Task DeleteClaimAsync(Claim claim, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Claim>> GetClaimsByCoverIdAsync(Guid coverId, CancellationToken cancellationToken = default);
}
