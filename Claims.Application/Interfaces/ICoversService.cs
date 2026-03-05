using Claims.Application.DTOs;
using Claims.Application.Requests.Cover;
using Claims.Domain.Enums;

namespace Claims.Application.Interfaces;

public interface ICoversService
{
    Task<IReadOnlyList<CoverDto>> GetCoversAsync();
    Task<CoverDto?> GetCoverAsync(string id);
    Task<CoverDto> CreateCoverAsync(CreateCoverRequest request);
    Task DeleteCoverAsync(string id);
    Task<decimal> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType);
}
