using Claims.Domain.Interfaces;
using Claims.Domain.Models;

namespace Claims.Infrastructure.Repositories;

public class ClaimsRepository : IClaimsRepository
{
    public Task CreateClaimAsync(Claim claim)
    {
        throw new NotImplementedException();
    }

    public Task DeleteClaimAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Claim?> GetClaimAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        throw new NotImplementedException();
    }
}
