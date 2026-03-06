using Claims.Application.Interfaces;
using Claims.Application.Requests.Cover;
using Claims.Domain.Enums;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;

namespace Claims.Application.Services;

public class CoversService(ICoversRepository _coversRepository, IAuditService _auditService,
    IPremiumCalculator _premiumCalculator) : ICoversService
{
    public async Task<IReadOnlyList<Cover>> GetCoversAsync()
    {
        var covers = await _coversRepository.GetCoversAsync();
        return covers.ToList();
    }

    public async Task<Cover?> GetCoverAsync(string id)
    {
        return await _coversRepository.GetCoverAsync(id);
    }

    public async Task<Cover> CreateCoverAsync(CreateCoverRequest request)
    {
        var cover = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = request.Type,
            Premium = await _premiumCalculator.ComputeAsync(request.StartDate, request.EndDate, request.Type)
        };

        await _coversRepository.CreateCoverAsync(cover);
        await _auditService.AuditCoverAsync(cover.Id, "POST");

        return cover;
    }

    public async Task DeleteCoverAsync(string id)
    {
        await _coversRepository.DeleteCoverAsync(id);
        await _auditService.AuditCoverAsync(id, "DELETE");
    }

    public Task<decimal> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
        => _premiumCalculator.ComputeAsync(startDate, endDate, coverType);
}