using Claims.Application.Interfaces;
using Claims.Application.Requests.Cover;
using Claims.Domain.Enums;
using Claims.Domain.Exceptions;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;

namespace Claims.Application.Services;

public class CoversService(ICoversRepository coversRepository, IClaimsRepository claimsRepository,
    IAuditService auditService, IPremiumCalculator premiumCalculator) : ICoversService
{
    public async Task<IReadOnlyList<Cover>> GetCoversAsync(CancellationToken cancellationToken = default)
        => await coversRepository.GetCoversAsync(cancellationToken);

    public async Task<Cover?> GetCoverByIdAsync(string id, CancellationToken cancellationToken = default)
        => await coversRepository.GetCoverByIdAsync(id, cancellationToken);

    public async Task<Cover> CreateCoverAsync(CreateCoverRequest request, CancellationToken cancellationToken = default)
    {
        var cover = new Cover
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = request.Type,
            Premium = premiumCalculator.Compute(request.StartDate, request.EndDate, request.Type)
        };
        await coversRepository.CreateCoverAsync(cover, cancellationToken);
        await auditService.AuditCoverAsync(cover.Id, HttpRequestType.POST);

        return cover;
    }

    public async Task DeleteCoverAsync(string id, CancellationToken cancellationToken = default)
    {
        var cover = await coversRepository.GetCoverByIdAsync(id, cancellationToken);
        if (cover is null)
            throw new CoverNotFoundException(id);

        var claims = await claimsRepository.GetClaimsByCoverIdAsync(id, cancellationToken);
        if (claims.Any())
            throw new CoverHasActiveClaimsException(id);

        await coversRepository.DeleteCoverAsync(cover, cancellationToken);
        await auditService.AuditCoverAsync(id, HttpRequestType.DELETE);
    }

    public decimal ComputePremium(ComputePremiumRequest request)
        => premiumCalculator.Compute(request.StartDate, request.EndDate, request.Type);
}
