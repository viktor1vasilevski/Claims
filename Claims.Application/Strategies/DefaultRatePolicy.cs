using Claims.Application.Constants;
using Claims.Domain.Enums;

namespace Claims.Application.Strategies;

/// <summary>
/// Fallback rate policy used for cover types without a dedicated policy (ContainerShip, BulkCarrier).
/// </summary>
public class DefaultRatePolicy : ICoverRatePolicy
{
    private readonly CoverType _coverType;

    public DefaultRatePolicy(CoverType coverType)
    {
        _coverType = coverType;
    }

    public CoverType CoverType => _coverType;
    public decimal Multiplier => PremiumConstants.DefaultMultiplier;
    public decimal FirstPeriodDiscount => PremiumConstants.DefaultFirstDiscount;
    public decimal SecondPeriodDiscount => PremiumConstants.DefaultSecondDiscount;
}
