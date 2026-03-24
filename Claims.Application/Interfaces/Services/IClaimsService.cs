namespace Claims.Application.Interfaces.Services;

public interface IClaimsService
{
    Task<IReadOnlyList<ClaimDto>> GetClaimsAsync(CancellationToken cancellationToken = default);
    Task<ClaimDto?> GetClaimByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ClaimDto> CreateClaimAsync(CreateClaimRequest request, CancellationToken cancellationToken = default);
    Task DeleteClaimAsync(Guid id, CancellationToken cancellationToken = default);
}
