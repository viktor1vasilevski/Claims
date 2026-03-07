using Claims.Domain.Exceptions;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using Claims.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

public class ClaimsRepository(ClaimsContext _context) : IClaimsRepository
{
    public async Task<IEnumerable<Claim>> GetClaimsAsync()
        => await _context.Claims.ToListAsync();

    public async Task<Claim?> GetClaimAsync(string id)
        => await _context.Claims.Where(c => c.Id == id).SingleOrDefaultAsync();

    public async Task CreateClaimAsync(Claim claim)
    {
        _context.Claims.Add(claim);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteClaimAsync(string id)
    {
        var claim = await GetClaimAsync(id);
        if (claim is null)
            throw new ClaimNotFoundException(id);

        _context.Claims.Remove(claim);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Claim>> GetClaimsByCoverIdAsync(string coverId)
        => await _context.Claims.Where(c => c.CoverId == coverId).ToListAsync();
}