using Claims.Application.Constants;
using Claims.Domain.Enums;

namespace Claims.Application.Strategies;

public class PassengerShipRateStrategy : DefaultRateStrategy
{
    public override CoverType CoverType => CoverType.PassengerShip;
    public override decimal GetMultiplier() => PremiumConstants.PassengerShipMultiplier;
}
