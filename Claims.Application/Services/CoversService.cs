using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Application.Mappers;
using Claims.Application.Requests.Cover;
using Claims.Domain.Enums;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;

namespace Claims.Application.Services;

public class CoversService(ICoversRepository _coversRepository, IAuditService _auditService) : ICoversService
{
    public async Task<IEnumerable<CoverDto>> GetCoversAsync()
    {
        var covers = await _coversRepository.GetCoversAsync();
        return covers.Select(CoverMapper.ToDto);
    }

    public async Task<CoverDto?> GetCoverAsync(string id)
    {
        var cover = await _coversRepository.GetCoverAsync(id);
        return cover is null ? null : CoverMapper.ToDto(cover);
    }

    public async Task<CoverDto> CreateCoverAsync(CreateCoverRequest request)
    {
        var cover = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = request.Type,
            Premium = ComputePremium(request.StartDate, request.EndDate, request.Type)
        };
        await _coversRepository.CreateCoverAsync(cover);
        await _auditService.AuditCoverAsync(cover.Id, "POST");
        return CoverMapper.ToDto(cover);
    }

    public async Task DeleteCoverAsync(string id)
    {
        await _coversRepository.DeleteCoverAsync(id);
        await _auditService.AuditCoverAsync(id, "DELETE");
    }

    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var multiplier = coverType switch
        {
            CoverType.Yacht => 1.1m,
            CoverType.PassengerShip => 1.2m,
            CoverType.Tanker => 1.5m,
            _ => 1.3m
        };

        var premiumPerDay = 1250 * multiplier;
        var insuranceLength = (endDate - startDate).TotalDays;
        var totalPremium = 0m;

        for (var i = 0; i < insuranceLength; i++)
        {
            if (i < 30)
            {
                totalPremium += premiumPerDay;
            }
            else if (i < 180)
            {
                var discount = coverType == CoverType.Yacht ? 0.05m : 0.02m;
                totalPremium += premiumPerDay - premiumPerDay * discount;
            }
            else
            {
                var discount = coverType == CoverType.Yacht ? 0.08m : 0.03m;
                totalPremium += premiumPerDay - premiumPerDay * discount;
            }
        }
        return totalPremium;
    }
}