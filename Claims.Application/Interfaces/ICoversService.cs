using Claims.Application.Requests.Cover;
using Claims.Domain.Enums;
using Claims.Domain.Models;

namespace Claims.Application.Interfaces;

public interface ICoversService
{
    Task<IReadOnlyList<Cover>> GetCoversAsync();
    Task<Cover?> GetCoverAsync(string id);
    Task<Cover> CreateCoverAsync(CreateCoverRequest request);
    Task DeleteCoverAsync(string id);
    Task<decimal> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType);
}
