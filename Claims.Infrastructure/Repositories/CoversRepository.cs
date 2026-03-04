using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using Claims.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

public class CoversRepository(ClaimsContext _context) : ICoversRepository
{

    public async Task<Cover?> GetCoverAsync(string id)
        => await _context.Covers.Where(c => c.Id == id).SingleOrDefaultAsync();

    public async Task<IEnumerable<Cover>> GetCoversAsync()
        => await _context.Covers.ToListAsync();

    public async Task CreateCoverAsync(Cover cover)
    {
        _context.Covers.Add(cover);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCoverAsync(string id)
    {
        var cover = await GetCoverAsync(id);
        if (cover is not null)
        {
            _context.Covers.Remove(cover);
            await _context.SaveChangesAsync();
        }
    }
}
