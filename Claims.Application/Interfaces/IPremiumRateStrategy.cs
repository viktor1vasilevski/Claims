using Claims.Domain.Enums;

namespace Claims.Application.Interfaces;

public interface IPremiumRateStrategy
{
    CoverType CoverType { get; }
    decimal GetMultiplier();
    decimal GetDiscount(int dayIndex);
}
