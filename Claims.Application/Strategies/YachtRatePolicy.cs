using Claims.Application.Constants;
using Claims.Domain.Enums;

namespace Claims.Application.Strategies;

public class YachtRatePolicy : ICoverRatePolicy
{
    public CoverType CoverType => CoverType.Yacht;
    public decimal Multiplier => PremiumConstants.YachtMultiplier;
    public decimal FirstPeriodDiscount => PremiumConstants.YachtFirstDiscount;
    public decimal SecondPeriodDiscount => PremiumConstants.YachtSecondDiscount;
}
