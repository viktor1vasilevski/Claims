using Claims.Application.Constants;
using Claims.Application.Interfaces;
using Claims.Domain.Enums;

namespace Claims.Application.Strategies;

public class YachtRateStrategy : IPremiumRateStrategy
{
    public CoverType CoverType => CoverType.Yacht;

    public decimal GetMultiplier() => PremiumConstants.YachtMultiplier;

    public decimal GetDiscount(int dayIndex) => dayIndex switch
    {
        < PremiumConstants.FirstPeriodDays => 0m,
        < PremiumConstants.SecondPeriodDays => PremiumConstants.YachtFirstDiscount,
        _ => PremiumConstants.YachtSecondDiscount
    };
}
