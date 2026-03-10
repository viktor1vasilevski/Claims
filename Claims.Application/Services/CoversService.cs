using Claims.Application.Interfaces;
using Claims.Application.Requests.Cover;
using Claims.Domain.Enums;
using Claims.Domain.Exceptions;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;

namespace Claims.Application.Services;

public class CoversService(ICoversRepository _coversRepository, IClaimsRepository _claimsRepository,
    IAuditService _auditService, IPremiumCalculator _premiumCalculator) : ICoversService
{
    public async Task<IReadOnlyList<Cover>> GetCoversAsync(CancellationToken cancellationToken = default)
        => await _coversRepository.GetCoversAsync(cancellationToken);

    public async Task<Cover?> GetCoverByIdAsync(string id, CancellationToken cancellationToken = default)
        => await _coversRepository.GetCoverByIdAsync(id, cancellationToken);

    public async Task<Cover> CreateCoverAsync(CreateCoverRequest request, CancellationToken cancellationToken = default)
    {
        var cover = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = request.Type,
            Premium = await _premiumCalculator.ComputeAsync(request.StartDate, request.EndDate, request.Type)
        };
        await _coversRepository.CreateCoverAsync(cover, cancellationToken);
        await _auditService.AuditCoverAsync(cover.Id, HttpRequestType.POST);

        return cover;
    }

    public async Task DeleteCoverAsync(string id, CancellationToken cancellationToken = default)
    {
        var cover = await _coversRepository.GetCoverByIdAsync(id, cancellationToken);
        if (cover is null)
            throw new CoverNotFoundException(id);

        var claims = await _claimsRepository.GetClaimsByCoverIdAsync(id, cancellationToken);
        if (claims.Any())
            throw new CoverHasActiveClaimsException(id);

        await _coversRepository.DeleteCoverAsync(cover, cancellationToken);
        await _auditService.AuditCoverAsync(id, HttpRequestType.DELETE);
    }

    public Task<decimal> ComputePremiumAsync(ComputePremiumRequest request)
        => _premiumCalculator.ComputeAsync(request.StartDate, request.EndDate, request.Type);
}