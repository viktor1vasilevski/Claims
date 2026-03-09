namespace Claims.Application.Constants;

public static class PremiumConstants
{
    public const decimal BaseDayRate = 1250m;

    public const int FirstPeriodDays = 30;
    public const int SecondPeriodDays = 150;

    public const decimal YachtMultiplier = 1.1m;
    public const decimal PassengerShipMultiplier = 1.2m;
    public const decimal TankerMultiplier = 1.5m;
    public const decimal DefaultMultiplier = 1.3m;

    public const decimal YachtFirstDiscount = 0.05m;
    public const decimal YachtAdditionalDiscount = 0.03m;

    public const decimal DefaultFirstDiscount = 0.02m;
    public const decimal DefaultAdditionalDiscount = 0.01m;
}
