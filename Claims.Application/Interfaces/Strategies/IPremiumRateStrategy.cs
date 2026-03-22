namespace Claims.Application.Interfaces.Strategies;

public interface IPremiumRateStrategy
{
    CoverType CoverType { get; }
    decimal GetMultiplier();
    decimal GetDiscount(int dayIndex);
}
