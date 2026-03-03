using Claims.Domain.Models;

namespace Claims.Domain.Interfaces;

public interface ICoversRepository
{
    Task<IEnumerable<Cover>> GetCoversAsync();
    Task<Cover?> GetCoverAsync(string id);
    Task CreateCoverAsync(Cover cover);
    Task DeleteCoverAsync(string id);
}
