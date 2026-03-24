namespace Claims.Application.Services;

public class ClaimsService(IClaimsRepository claimsRepository, IAuditService auditService,
    ICoversRepository coversRepository) : IClaimsService
{
    public async Task<IReadOnlyList<ClaimDto>> GetClaimsAsync(CancellationToken cancellationToken = default)
    {
        var claims = await claimsRepository.GetClaimsAsync(cancellationToken);
        return claims.Select(ClaimMapper.ToDto).ToList();
    }

    public async Task<ClaimDto?> GetClaimByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var claim = await claimsRepository.GetClaimByIdAsync(id, cancellationToken);
        return claim is null ? null : ClaimMapper.ToDto(claim);
    }

    public async Task<ClaimDto> CreateClaimAsync(CreateClaimRequest request, CancellationToken cancellationToken = default)
    {
        var cover = await coversRepository.GetCoverByIdAsync(request.CoverId, cancellationToken);

        if (cover is null)
            throw new CoverNotFoundException(request.CoverId);

        if (!cover.IsDateWithinPeriod(request.Created))
            throw new ClaimDateOutOfRangeException();

        var claim = Claim.Create(request.CoverId, request.Name, request.Type, request.DamageCost, request.Created);

        await claimsRepository.CreateClaimAsync(claim, cancellationToken);
        await auditService.AuditClaimAsync(claim.Id.ToString(), HttpRequestType.POST);

        return ClaimMapper.ToDto(claim);
    }

    public async Task DeleteClaimAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var claim = await claimsRepository.GetClaimByIdAsync(id, cancellationToken);
        if (claim is null)
            throw new ClaimNotFoundException(id);
        await claimsRepository.DeleteClaimAsync(claim, cancellationToken);
        await auditService.AuditClaimAsync(id.ToString(), HttpRequestType.DELETE);
    }
}
