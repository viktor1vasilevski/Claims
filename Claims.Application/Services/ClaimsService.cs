using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Application.Mappers;
using Claims.Application.Requests.Claims;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;

namespace Claims.Application.Services;

public class ClaimsService(IClaimsRepository _claimsRepository, IAuditService _auditService, 
    ICoversRepository _coversRepository) : IClaimsService
{
    public async Task<List<ClaimDto>> GetClaimsAsync()
    {
        var claims = await _claimsRepository.GetClaimsAsync();
        return claims.Select(ClaimMapper.ToDto).ToList();
    }

    public async Task<ClaimDto?> GetClaimAsync(string id)
    {
        var claim = await _claimsRepository.GetClaimAsync(id);
        return claim is null ? null : ClaimMapper.ToDto(claim);
    }

    public async Task<ClaimDto> CreateClaimAsync(CreateClaimRequest request)
    {
        var cover = await _coversRepository.GetCoverAsync(request.CoverId);
        if (cover is null)
            throw new ArgumentException("Cover not found.");

        if (request.Created < cover.StartDate || request.Created > cover.EndDate)
            throw new ArgumentException("Created date must be within the Cover period.");

        var claim = new Claim
        {
            Id = Guid.NewGuid().ToString(),
            CoverId = request.CoverId,
            Created = request.Created,
            Name = request.Name,
            Type = request.Type,
            DamageCost = request.DamageCost
        };

        await _claimsRepository.CreateClaimAsync(claim);
        await _auditService.AuditClaimAsync(claim.Id, "POST");

        return ClaimMapper.ToDto(claim);
    }

    public async Task DeleteClaimAsync(string id)
    {
        await _claimsRepository.DeleteClaimAsync(id);
        await _auditService.AuditClaimAsync(id, "DELETE");
    }
}