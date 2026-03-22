namespace Claims.Application.Interfaces.Services;

public interface ICoversService
{
    Task<IReadOnlyList<Cover>> GetCoversAsync(CancellationToken cancellationToken = default);
    Task<Cover?> GetCoverByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Cover> CreateCoverAsync(CreateCoverRequest request, CancellationToken cancellationToken = default);
    Task DeleteCoverAsync(Guid id, CancellationToken cancellationToken = default);
    decimal ComputePremium(ComputePremiumRequest request);
}
