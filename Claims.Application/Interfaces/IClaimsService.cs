using Claims.Application.Requests.Claims;
using Claims.Domain.Models;

namespace Claims.Application.Interfaces;

public interface IClaimsService
{
    Task<IReadOnlyList<Claim>> GetClaimsAsync();
    Task<Claim?> GetClaimAsync(string id);
    Task<Claim> CreateClaimAsync(CreateClaimRequest request);
    Task DeleteClaimAsync(string id);
}
