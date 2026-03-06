using Claims.Domain.Enums;

namespace Claims.Application.Strategies;

public class ContainerShipRateStrategy : DefaultRateStrategy
{
    public override CoverType CoverType => CoverType.ContainerShip;
}
