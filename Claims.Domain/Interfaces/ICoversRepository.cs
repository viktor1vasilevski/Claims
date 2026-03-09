using Claims.Domain.Models;

namespace Claims.Domain.Interfaces;

public interface ICoversRepository
{
    Task<IReadOnlyList<Cover>> GetCoversAsync(CancellationToken cancellationToken = default);
    Task<Cover?> GetCoverByIdAsync(string id, CancellationToken cancellationToken = default);
    Task CreateCoverAsync(Cover cover, CancellationToken cancellationToken = default);
    Task DeleteCoverAsync(string id, CancellationToken cancellationToken = default);
}
