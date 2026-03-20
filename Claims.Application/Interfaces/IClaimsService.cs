using Claims.Application.Requests.Claims;
using Claims.Domain.Models;

namespace Claims.Application.Interfaces;

public interface IClaimsService
{
    Task<IReadOnlyList<Claim>> GetClaimsAsync(CancellationToken cancellationToken = default);
    Task<Claim?> GetClaimByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Claim> CreateClaimAsync(CreateClaimRequest request, CancellationToken cancellationToken = default);
    Task DeleteClaimAsync(Guid id, CancellationToken cancellationToken = default);
}
