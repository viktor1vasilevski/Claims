namespace Claims.Application.Services;

public class PremiumCalculator(IEnumerable<IPremiumRateStrategy> _strategies) : IPremiumCalculator
{
    public decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var strategy = _strategies.FirstOrDefault(s => s.CoverType == coverType)
            ?? throw new PremiumStrategyNotFoundException(coverType);

        var premiumPerDay = PremiumConstants.BaseDayRate * strategy.GetMultiplier();
        var insuranceLength = (int)(endDate - startDate).TotalDays;
        var totalPremium = 0m;

        for (var i = 0; i < insuranceLength; i++)
        {
            var discount = strategy.GetDiscount(i);
            totalPremium += premiumPerDay - premiumPerDay * discount;
        }

        return totalPremium;
    }
}
