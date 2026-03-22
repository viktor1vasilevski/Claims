namespace Claims.Application.Strategies;

public class YachtRateStrategy : DefaultRateStrategy
{
    public override CoverType CoverType => CoverType.Yacht;

    public override decimal GetMultiplier() => PremiumConstants.YachtMultiplier;

    public override decimal GetDiscount(int dayIndex) => dayIndex switch
    {
        < PremiumConstants.FirstPeriodDays => 0m,
        < PremiumConstants.FirstPeriodDays + PremiumConstants.SecondPeriodDays => PremiumConstants.YachtFirstDiscount,
        _ => PremiumConstants.YachtFirstDiscount + PremiumConstants.YachtAdditionalDiscount
    };
}
