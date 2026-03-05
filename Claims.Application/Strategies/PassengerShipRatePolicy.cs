using Claims.Application.Constants;
using Claims.Domain.Enums;

namespace Claims.Application.Strategies;

public class PassengerShipRatePolicy : ICoverRatePolicy
{
    public CoverType CoverType => CoverType.PassengerShip;
    public decimal Multiplier => PremiumConstants.PassengerShipMultiplier;
    public decimal FirstPeriodDiscount => PremiumConstants.DefaultFirstDiscount;
    public decimal SecondPeriodDiscount => PremiumConstants.DefaultSecondDiscount;
}
