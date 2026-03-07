using Claims.Domain.Exceptions;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using Claims.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

public class CoversRepository(ClaimsContext _context) : ICoversRepository
{
    public async Task<Cover?> GetCoverAsync(string id, CancellationToken cancellationToken = default)
        => await _context.Covers.Where(c => c.Id == id).SingleOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<Cover>> GetCoversAsync(CancellationToken cancellationToken = default)
        => await _context.Covers.ToListAsync(cancellationToken);

    public async Task CreateCoverAsync(Cover cover, CancellationToken cancellationToken = default)
    {
        _context.Covers.Add(cover);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCoverAsync(string id, CancellationToken cancellationToken = default)
    {
        var cover = await GetCoverAsync(id, cancellationToken);
        if (cover is null)
            throw new CoverNotFoundException(id);
        _context.Covers.Remove(cover);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
