using Claims.Application.Requests.Cover;
using Claims.Domain.Models;

namespace Claims.Application.Interfaces;

public interface ICoversService
{
    Task<IReadOnlyList<Cover>> GetCoversAsync(CancellationToken cancellationToken = default);
    Task<Cover?> GetCoverByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Cover> CreateCoverAsync(CreateCoverRequest request, CancellationToken cancellationToken = default);
    Task DeleteCoverAsync(string id, CancellationToken cancellationToken = default);
    Task<decimal> ComputePremiumAsync(ComputePremiumRequest request);
}