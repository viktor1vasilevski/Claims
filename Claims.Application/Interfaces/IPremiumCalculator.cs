using Claims.Domain.Enums;

namespace Claims.Application.Interfaces;

public interface IPremiumCalculator
{
    Task<decimal> ComputeAsync(DateTime startDate, DateTime endDate, CoverType coverType);
}
