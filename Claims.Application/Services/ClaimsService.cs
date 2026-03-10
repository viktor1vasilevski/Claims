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
    public async Task<IReadOnlyList<Claim>> GetClaimsAsync(CancellationToken cancellationToken = default)
        => await _claimsRepository.GetClaimsAsync(cancellationToken);

    public async Task<Claim?> GetClaimByIdAsync(string id, CancellationToken cancellationToken = default)
        => await _claimsRepository.GetClaimByIdAsync(id, cancellationToken);

    public async Task<Claim> CreateClaimAsync(CreateClaimRequest request, CancellationToken cancellationToken = default)
    {
        var cover = await _coversRepository.GetCoverByIdAsync(request.CoverId, cancellationToken);

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

        await _claimsRepository.CreateClaimAsync(claim, cancellationToken);
        await _auditService.AuditClaimAsync(claim.Id, HttpRequestType.POST);

        return claim;
    }

    public async Task DeleteClaimAsync(string id, CancellationToken cancellationToken = default)
    {
        var claim = await _claimsRepository.GetClaimByIdAsync(id, cancellationToken);
        if (claim is null)
            throw new ClaimNotFoundException(id);
        await _claimsRepository.DeleteClaimAsync(claim, cancellationToken);
        await _auditService.AuditClaimAsync(id, HttpRequestType.DELETE);
    }
}