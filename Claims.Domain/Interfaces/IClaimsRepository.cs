using Claims.Domain.Models;

namespace Claims.Domain.Interfaces;

public interface IClaimsRepository
{
    Task<IEnumerable<Claim>> GetClaimsAsync();
    Task<Claim?> GetClaimAsync(string id);
    Task CreateClaimAsync(Claim claim);
    Task DeleteClaimAsync(string id);
}
