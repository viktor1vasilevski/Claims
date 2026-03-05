using Claims.Application.Constants;
using Claims.Application.Interfaces;
using Claims.Application.Strategies;
using Claims.Domain.Enums;

namespace Claims.Application.Services;

public class PremiumCalculator(IEnumerable<ICoverRatePolicy> _policies) : IPremiumCalculator
{
    private readonly Dictionary<CoverType, ICoverRatePolicy> _policyMap =
        _policies.ToDictionary(p => p.CoverType);

    public Task<decimal> ComputeAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        if (!_policyMap.TryGetValue(coverType, out var policy))
            throw new ArgumentException($"No rate policy registered for cover type '{coverType}'.");

        var premiumPerDay = PremiumConstants.BaseDayRate * policy.Multiplier;
        var insuranceLength = (endDate - startDate).TotalDays;
        var totalPremium = 0m;

        for (var i = 0; i < insuranceLength; i++)
        {
            if (i < PremiumConstants.FirstPeriodDays)
                totalPremium += premiumPerDay;
            else if (i < PremiumConstants.SecondPeriodDays)
                totalPremium += premiumPerDay - premiumPerDay * policy.FirstPeriodDiscount;
            else
                totalPremium += premiumPerDay - premiumPerDay * policy.SecondPeriodDiscount;
        }

        return Task.FromResult(totalPremium);
    }
}
