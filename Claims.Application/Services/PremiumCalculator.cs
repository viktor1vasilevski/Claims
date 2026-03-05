using Claims.Application.Constants;
using Claims.Application.Interfaces;
using Claims.Domain.Enums;

namespace Claims.Application.Services;

public class PremiumCalculator : IPremiumCalculator
{
    public Task<decimal> ComputeAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var multiplier = coverType switch
        {
            CoverType.Yacht => PremiumConstants.YachtMultiplier,
            CoverType.PassengerShip => PremiumConstants.PassengerShipMultiplier,
            CoverType.Tanker => PremiumConstants.TankerMultiplier,
            _ => PremiumConstants.DefaultMultiplier
        };

        var premiumPerDay = PremiumConstants.BaseDayRate * multiplier;
        var insuranceLength = (endDate - startDate).TotalDays;
        var totalPremium = 0m;

        for (var i = 0; i < insuranceLength; i++)
        {
            if (i < PremiumConstants.FirstPeriodDays)
                totalPremium += premiumPerDay;
            else if (i < PremiumConstants.SecondPeriodDays)
            {
                var discount = coverType == CoverType.Yacht ? PremiumConstants.YachtFirstDiscount : PremiumConstants.DefaultFirstDiscount;
                totalPremium += premiumPerDay - premiumPerDay * discount;
            }
            else
            {
                var discount = coverType == CoverType.Yacht ? PremiumConstants.YachtSecondDiscount : PremiumConstants.DefaultSecondDiscount;
                totalPremium += premiumPerDay - premiumPerDay * discount;
            }
        }

        return Task.FromResult(totalPremium);
    }
}
