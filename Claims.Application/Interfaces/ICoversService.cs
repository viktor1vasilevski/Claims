using Claims.Application.Requests.Cover;
using Claims.Domain.Enums;
using Claims.Domain.Models;

namespace Claims.Application.Interfaces;

public interface ICoversService
{
    Task<IReadOnlyList<Cover>> GetCoversAsync(CancellationToken cancellationToken = default);
    Task<Cover?> GetCoverAsync(string id, CancellationToken cancellationToken = default);
    Task<Cover> CreateCoverAsync(CreateCoverRequest request, CancellationToken cancellationToken = default);
    Task DeleteCoverAsync(string id, CancellationToken cancellationToken = default);
    Task<decimal> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType);
}