using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Application.Requests.Cover;
using Claims.Domain.Enums;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using FluentValidation;

namespace Claims.Application.Services;

public class CoversService(ICoversRepository _coversRepository, IAuditService _auditService) : ICoversService
{

    public async Task<IEnumerable<CoverDto>> GetCoversAsync()
    {
        var covers = await _coversRepository.GetCoversAsync();
        return covers.Select(MapToDto);
    }

    public async Task<CoverDto?> GetCoverAsync(string id)
    {
        var cover = await _coversRepository.GetCoverAsync(id);
        return cover is null ? null : MapToDto(cover);
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

        return MapToDto(cover);
    }

    public async Task DeleteCoverAsync(string id)
    {
        await _coversRepository.DeleteCoverAsync(id);
        await _auditService.AuditCoverAsync(id, "DELETE");
    }

    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var multiplier = 1.3m;
        if (coverType == CoverType.Yacht) multiplier = 1.1m;
        if (coverType == CoverType.PassengerShip) multiplier = 1.2m;
        if (coverType == CoverType.Tanker) multiplier = 1.5m;

        var premiumPerDay = 1250 * multiplier;
        var insuranceLength = (endDate - startDate).TotalDays;
        var totalPremium = 0m;

        for (var i = 0; i < insuranceLength; i++)
        {
            if (i < 30) totalPremium += premiumPerDay;
            if (i < 180 && coverType == CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.05m;
            else if (i < 180) totalPremium += premiumPerDay - premiumPerDay * 0.02m;
            if (i < 365 && coverType != CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.03m;
            else if (i < 365) totalPremium += premiumPerDay - premiumPerDay * 0.08m;
        }
        return totalPremium;
    }

    private static CoverDto MapToDto(Cover cover) => new()
    {
        Id = cover.Id,
        StartDate = cover.StartDate,
        EndDate = cover.EndDate,
        Type = cover.Type,
        Premium = cover.Premium
    };
}
