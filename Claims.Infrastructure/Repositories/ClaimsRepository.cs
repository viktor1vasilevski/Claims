using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using Claims.Infrastructure.Context;

namespace Claims.Infrastructure.Repositories;

public class ClaimsRepository(ClaimsContext _context) : IClaimsRepository
{

    public async Task<IEnumerable<Claim>> GetClaimsAsync()
        => await _context.GetClaimsAsync();

    public async Task<Claim?> GetClaimAsync(string id)
        => await _context.GetClaimAsync(id);

    public async Task CreateClaimAsync(Claim claim)
    {
        claim.Id = Guid.NewGuid().ToString();
        await _context.AddItemAsync(claim);
    }

    public async Task DeleteClaimAsync(string id)
        => await _context.DeleteItemAsync(id);
}