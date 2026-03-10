using Claims.Application.Interfaces;
using Claims.Application.Requests.Claims;
using Claims.Domain.Enums;
using Claims.Domain.Exceptions;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;

namespace Claims.Application.Services;

public class ClaimsService(IClaimsRepository claimsRepository, IAuditService auditService,
    ICoversRepository coversRepository) : IClaimsService
{
    public async Task<IReadOnlyList<Claim>> GetClaimsAsync(CancellationToken cancellationToken = default)
        => await claimsRepository.GetClaimsAsync(cancellationToken);

    public async Task<Claim?> GetClaimByIdAsync(string id, CancellationToken cancellationToken = default)
        => await claimsRepository.GetClaimByIdAsync(id, cancellationToken);

    public async Task<Claim> CreateClaimAsync(CreateClaimRequest request, CancellationToken cancellationToken = default)
    {
        var cover = await coversRepository.GetCoverByIdAsync(request.CoverId, cancellationToken);

        if (cover is null)
            throw new CoverNotFoundException(request.CoverId);

        if (request.Created < cover.StartDate || request.Created > cover.EndDate)
            throw new ClaimDateOutOfRangeException();

        var claim = new Claim
        {
            CoverId = request.CoverId,
            Created = request.Created,
            Name = request.Name,
            Type = request.Type,
            DamageCost = request.DamageCost
        };

        await claimsRepository.CreateClaimAsync(claim, cancellationToken);
        await auditService.AuditClaimAsync(claim.Id, HttpRequestType.POST);

        return claim;
    }

    public async Task DeleteClaimAsync(string id, CancellationToken cancellationToken = default)
    {
        var claim = await claimsRepository.GetClaimByIdAsync(id, cancellationToken);
        if (claim is null)
            throw new ClaimNotFoundException(id);
        await claimsRepository.DeleteClaimAsync(claim, cancellationToken);
        await auditService.AuditClaimAsync(id, HttpRequestType.DELETE);
    }
}
