using Claims.Domain.Enums;

namespace Claims.Application.Strategies;

public class BulkCarrierRateStrategy : DefaultRateStrategy
{
    public override CoverType CoverType => CoverType.BulkCarrier;
}
