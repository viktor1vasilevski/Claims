using Claims.Application.Constants;
using Claims.Domain.Enums;

namespace Claims.Application.Strategies;

public class TankerRateStrategy : DefaultRateStrategy
{
    public override CoverType CoverType => CoverType.Tanker;
    public override decimal GetMultiplier() => PremiumConstants.TankerMultiplier;
}
