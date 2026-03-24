namespace Claims.Application.Services;

public class CoversService(ICoversRepository coversRepository, IClaimsRepository claimsRepository,
    IAuditService auditService, IPremiumCalculator premiumCalculator) : ICoversService
{
    public async Task<IReadOnlyList<CoverDto>> GetCoversAsync(CancellationToken cancellationToken = default)
    {
        var covers = await coversRepository.GetCoversAsync(cancellationToken);
        return covers.Select(CoverMapper.ToDto).ToList();
    }

    public async Task<CoverDto?> GetCoverByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cover = await coversRepository.GetCoverByIdAsync(id, cancellationToken);
        return cover is null ? null : CoverMapper.ToDto(cover);
    }

    public async Task<CoverDto> CreateCoverAsync(CreateCoverRequest request, CancellationToken cancellationToken = default)
    {
        var premium = premiumCalculator.Compute(request.StartDate, request.EndDate, request.Type);
        var cover = Cover.Create(request.StartDate, request.EndDate, request.Type, premium);

        await coversRepository.CreateCoverAsync(cover, cancellationToken);
        await auditService.AuditCoverAsync(cover.Id.ToString(), HttpRequestType.POST);

        return CoverMapper.ToDto(cover);
    }

    public async Task DeleteCoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cover = await coversRepository.GetCoverByIdAsync(id, cancellationToken);
        if (cover is null)
            throw new CoverNotFoundException(id);

        var claims = await claimsRepository.GetClaimsByCoverIdAsync(id, cancellationToken);
        if (claims.Any())
            throw new CoverHasActiveClaimsException(id);

        await coversRepository.DeleteCoverAsync(cover, cancellationToken);
        await auditService.AuditCoverAsync(id.ToString(), HttpRequestType.DELETE);
    }

    public decimal ComputePremium(ComputePremiumRequest request)
        => premiumCalculator.Compute(request.StartDate, request.EndDate, request.Type);
}
