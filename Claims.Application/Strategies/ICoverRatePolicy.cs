using Claims.Domain.Enums;

namespace Claims.Application.Strategies;

public interface ICoverRatePolicy
{
    CoverType CoverType { get; }
    decimal Multiplier { get; }
    decimal FirstPeriodDiscount { get; }
    decimal SecondPeriodDiscount { get; }
}
