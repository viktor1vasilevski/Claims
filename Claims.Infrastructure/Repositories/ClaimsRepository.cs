using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using Claims.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

public class ClaimsRepository(ClaimsContext context) : IClaimsRepository
{
    public async Task<IReadOnlyList<Claim>> GetClaimsAsync(CancellationToken cancellationToken = default)
        => await context.Claims.ToListAsync(cancellationToken);

    public async Task<Claim?> GetClaimByIdAsync(string id, CancellationToken cancellationToken = default)
        => await context.Claims.FindAsync(id, cancellationToken);

    public async Task CreateClaimAsync(Claim claim, CancellationToken cancellationToken = default)
    {
        context.Claims.Add(claim);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteClaimAsync(Claim claim, CancellationToken cancellationToken = default)
    {
        context.Claims.Remove(claim);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Claim>> GetClaimsByCoverIdAsync(string coverId, CancellationToken cancellationToken = default)
        => await context.Claims.Where(c => c.CoverId == coverId).ToListAsync(cancellationToken);
}
