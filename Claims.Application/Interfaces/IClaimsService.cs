using Claims.Application.Requests.Claims;
using Claims.Domain.Models;

namespace Claims.Application.Interfaces;

public interface IClaimsService
{
    Task<IReadOnlyList<Claim>> GetClaimsAsync(CancellationToken cancellationToken = default);
    Task<Claim?> GetClaimAsync(string id, CancellationToken cancellationToken = default);
    Task<Claim> CreateClaimAsync(CreateClaimRequest request, CancellationToken cancellationToken = default);
    Task DeleteClaimAsync(string id, CancellationToken cancellationToken = default);
}
