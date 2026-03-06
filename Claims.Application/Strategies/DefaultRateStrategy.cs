using Claims.Application.Constants;
using Claims.Application.Interfaces;
using Claims.Domain.Enums;

namespace Claims.Application.Strategies;

public abstract class DefaultRateStrategy : IPremiumRateStrategy
{
    public abstract CoverType CoverType { get; }
    public virtual decimal GetMultiplier() => PremiumConstants.DefaultMultiplier;
    public virtual decimal GetDiscount(int dayIndex) => dayIndex switch
    {
        < PremiumConstants.FirstPeriodDays => 0m,
        < PremiumConstants.SecondPeriodDays => PremiumConstants.DefaultFirstDiscount,
        _ => PremiumConstants.DefaultSecondDiscount
    };
}
