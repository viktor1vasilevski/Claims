using Claims.Application.Interfaces;
using Claims.Application.Requests.Claims;
using Claims.Domain.Enums;
using Claims.Domain.Exceptions;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;

namespace Claims.Application.Services;

public class ClaimsService(IClaimsRepository _claimsRepository, IAuditService _auditService, 
    ICoversRepository _coversRepository) : IClaimsService
{
    public async Task<IReadOnlyList<Claim>> GetClaimsAsync()
    {
        var claims = await _claimsRepository.GetClaimsAsync();
        return claims.ToList();
    }

    public async Task<Claim?> GetClaimAsync(string id)
    {
        return await _claimsRepository.GetClaimAsync(id);
    }

    public async Task<Claim> CreateClaimAsync(CreateClaimRequest request)
    {
        var cover = await _coversRepository.GetCoverAsync(request.CoverId);
        if (cover is null)
            throw new CoverNotFoundException(request.CoverId);

        if (request.Created < cover.StartDate || request.Created > cover.EndDate)
            throw new ClaimDateOutOfRangeException();

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
        await _auditService.AuditClaimAsync(claim.Id, HttpRequestType.POST);

        return claim;
    }

    public async Task DeleteClaimAsync(string id)
    {
        await _claimsRepository.DeleteClaimAsync(id);
        await _auditService.AuditClaimAsync(id, HttpRequestType.DELETE);
    }
}