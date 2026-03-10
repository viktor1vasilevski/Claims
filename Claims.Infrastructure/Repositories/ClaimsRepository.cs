using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using Claims.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

public class ClaimsRepository(ClaimsContext _context) : IClaimsRepository
{
    public async Task<IReadOnlyList<Claim>> GetClaimsAsync(CancellationToken cancellationToken = default)
        => await _context.Claims.ToListAsync(cancellationToken);

    public async Task<Claim?> GetClaimByIdAsync(string id, CancellationToken cancellationToken = default)
        => await _context.Claims.FindAsync(id, cancellationToken);

    public async Task CreateClaimAsync(Claim claim, CancellationToken cancellationToken = default)
    {
        _context.Claims.Add(claim);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteClaimAsync(Claim claim, CancellationToken cancellationToken = default)
    {
        _context.Claims.Remove(claim);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Claim>> GetClaimsByCoverIdAsync(string coverId, CancellationToken cancellationToken = default)
        => await _context.Claims.Where(c => c.CoverId == coverId).ToListAsync(cancellationToken);
}