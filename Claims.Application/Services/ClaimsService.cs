using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;

namespace Claims.Application.Services;

public class ClaimsService(IClaimsRepository _claimsRepository, IAuditService _auditService) : IClaimsService
{
    public async Task<List<ClaimDto>> GetClaimsAsync()
    {
        var claims = await _claimsRepository.GetClaimsAsync();
        return claims.Select(MapToDto).ToList();
    }

    public async Task<ClaimDto?> GetClaimAsync(string id)
    {
        var claim = await _claimsRepository.GetClaimAsync(id);
        return claim is null ? null : MapToDto(claim);
    }

    public async Task CreateClaimAsync(Claim claim)
    {
        await _claimsRepository.CreateClaimAsync(claim);
        await _auditService.AuditClaimAsync(claim.Id, "POST");
    }

    public async Task DeleteClaimAsync(string id)
    {
        await _claimsRepository.DeleteClaimAsync(id);
        await _auditService.AuditClaimAsync(id, "DELETE");
    }

    private static ClaimDto MapToDto(Claim claim) => new()
    {
        Id = claim.Id,
        CoverId = claim.CoverId,
        Created = claim.Created,
        Name = claim.Name,
        Type = claim.Type,
        DamageCost = claim.DamageCost
    };
}