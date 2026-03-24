namespace Claims.Application.Interfaces.Services;

public interface ICoversService
{
    Task<IReadOnlyList<CoverDto>> GetCoversAsync(CancellationToken cancellationToken = default);
    Task<CoverDto?> GetCoverByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CoverDto> CreateCoverAsync(CreateCoverRequest request, CancellationToken cancellationToken = default);
    Task DeleteCoverAsync(Guid id, CancellationToken cancellationToken = default);
    decimal ComputePremium(ComputePremiumRequest request);
}
