using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using Claims.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

public class CoversRepository(ClaimsContext _context) : ICoversRepository
{
    public async Task<IReadOnlyList<Cover>> GetCoversAsync(CancellationToken cancellationToken = default)
        => await _context.Covers.ToListAsync(cancellationToken);

    public async Task<Cover?> GetCoverByIdAsync(string id, CancellationToken cancellationToken = default)
        => await _context.Covers.FindAsync(id, cancellationToken);

    public async Task CreateCoverAsync(Cover cover, CancellationToken cancellationToken = default)
    {
        _context.Covers.Add(cover);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCoverAsync(Cover cover, CancellationToken cancellationToken = default)
    {
        _context.Covers.Remove(cover);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
