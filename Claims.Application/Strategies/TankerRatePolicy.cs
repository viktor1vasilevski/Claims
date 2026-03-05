using Claims.Application.Constants;
using Claims.Domain.Enums;

namespace Claims.Application.Strategies;

public class TankerRatePolicy : ICoverRatePolicy
{
    public CoverType CoverType => CoverType.Tanker;
    public decimal Multiplier => PremiumConstants.TankerMultiplier;
    public decimal FirstPeriodDiscount => PremiumConstants.DefaultFirstDiscount;
    public decimal SecondPeriodDiscount => PremiumConstants.DefaultSecondDiscount;
}
