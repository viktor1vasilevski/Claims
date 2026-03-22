namespace Claims.Application.Services;

public class ClaimsService(IClaimsRepository claimsRepository, IAuditService auditService,
    ICoversRepository coversRepository) : IClaimsService
{
    public async Task<IReadOnlyList<Claim>> GetClaimsAsync(CancellationToken cancellationToken = default)
        => await claimsRepository.GetClaimsAsync(cancellationToken);

    public async Task<Claim?> GetClaimByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await claimsRepository.GetClaimByIdAsync(id, cancellationToken);

    public async Task<Claim> CreateClaimAsync(CreateClaimRequest request, CancellationToken cancellationToken = default)
    {
        var cover = await coversRepository.GetCoverByIdAsync(request.CoverId, cancellationToken);

        if (cover is null)
            throw new CoverNotFoundException(request.CoverId);

        if (!cover.IsDateWithinPeriod(request.Created))
            throw new ClaimDateOutOfRangeException();

        var claim = Claim.Create(request.CoverId, request.Name, request.Type, request.DamageCost, request.Created);

        await claimsRepository.CreateClaimAsync(claim, cancellationToken);
        await auditService.AuditClaimAsync(claim.Id.ToString(), HttpRequestType.POST);

        return claim;
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
