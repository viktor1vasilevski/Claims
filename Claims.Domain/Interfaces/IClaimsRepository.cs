using Claims.Domain.Models;

namespace Claims.Domain.Interfaces;

public interface IClaimsRepository
{
    Task<IEnumerable<Claim>> GetClaimsAsync(CancellationToken cancellationToken = default);
    Task<Claim?> GetClaimAsync(string id, CancellationToken cancellationToken = default);
    Task CreateClaimAsync(Claim claim, CancellationToken cancellationToken = default);
    Task DeleteClaimAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Claim>> GetClaimsByCoverIdAsync(string coverId, CancellationToken cancellationToken = default);
}
