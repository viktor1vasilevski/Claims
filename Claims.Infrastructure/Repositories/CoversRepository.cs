using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using Claims.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

public class CoversRepository(ClaimsContext context) : ICoversRepository
{
    public async Task<IReadOnlyList<Cover>> GetCoversAsync(CancellationToken cancellationToken = default)
        => await context.Covers.ToListAsync(cancellationToken);

    public async Task<Cover?> GetCoverByIdAsync(string id, CancellationToken cancellationToken = default)
        => await context.Covers.FindAsync([id], cancellationToken);

    public async Task CreateCoverAsync(Cover cover, CancellationToken cancellationToken = default)
    {
        context.Covers.Add(cover);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCoverAsync(Cover cover, CancellationToken cancellationToken = default)
    {
        context.Covers.Remove(cover);
        await context.SaveChangesAsync(cancellationToken);
    }
}
