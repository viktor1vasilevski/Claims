
namespace Claims.Application.Tests.Services;

public class PremiumCalculatorTests
{
    private readonly PremiumCalculator _sut = new(
    [
        new YachtRateStrategy(),
        new TankerRateStrategy(),
        new PassengerShipRateStrategy(),
        new ContainerShipRateStrategy(),
        new BulkCarrierRateStrategy()
    ]);

    [Theory]
    // Single day - minimum valid period, no discount
    [InlineData(CoverType.BulkCarrier, 1, 1625)]
    [InlineData(CoverType.ContainerShip, 1, 1625)]
    [InlineData(CoverType.PassengerShip, 1, 1500)]
    [InlineData(CoverType.Tanker, 1, 1875)]
    [InlineData(CoverType.Yacht, 1, 1375)]
    // First period only - no discounts
    [InlineData(CoverType.BulkCarrier, 30, 48750)]
    [InlineData(CoverType.ContainerShip, 30, 48750)]
    [InlineData(CoverType.PassengerShip, 30, 45000)]
    [InlineData(CoverType.Tanker, 30, 56250)]
    [InlineData(CoverType.Yacht, 30, 41250)]
    // Cross into second period - first discount applies
    [InlineData(CoverType.BulkCarrier, 31, 50342.5)]
    [InlineData(CoverType.ContainerShip, 31, 50342.5)]
    [InlineData(CoverType.PassengerShip, 31, 46470)]
    [InlineData(CoverType.Tanker, 31, 58087.5)]
    [InlineData(CoverType.Yacht, 31, 42556.25)]
    // Last day of second period - only first discount applies, no additional discount yet
    [InlineData(CoverType.BulkCarrier, 180, 287625)]
    [InlineData(CoverType.ContainerShip, 180, 287625)]
    [InlineData(CoverType.PassengerShip, 180, 265500)]
    [InlineData(CoverType.Tanker, 180, 331875)]
    [InlineData(CoverType.Yacht, 180, 237187.5)]
    // Cross into third period - second discount applies
    [InlineData(CoverType.BulkCarrier, 181, 289201.25)]
    [InlineData(CoverType.ContainerShip, 181, 289201.25)]
    [InlineData(CoverType.PassengerShip, 181, 266955)]
    [InlineData(CoverType.Tanker, 181, 333693.75)]
    [InlineData(CoverType.Yacht, 181, 238452.5)]
    // Full 365-day period - maximum valid cover duration
    [InlineData(CoverType.BulkCarrier, 365, 579231.25)]
    [InlineData(CoverType.ContainerShip, 365, 579231.25)]
    [InlineData(CoverType.PassengerShip, 365, 534675)]
    [InlineData(CoverType.Tanker, 365, 668343.75)]
    [InlineData(CoverType.Yacht, 365, 471212.5)]
    public void Compute_ShouldReturnCorrectPremium(CoverType coverType, int days, decimal expectedPremium)
    {
        var startDate = new DateTime(2026, 1, 1);
        var endDate = startDate.AddDays(days);

        var result = _sut.Compute(startDate, endDate, coverType);

        result.Should().Be(expectedPremium);
    }

    [Fact]
    public void Compute_WhenStartAndEndDateAreEqual_ShouldReturnZero()
    {
        var date = new DateTime(2026, 1, 1);

        var result = _sut.Compute(date, date, CoverType.Yacht);

        result.Should().Be(0m);
    }

    [Fact]
    public void Compute_WhenDatesHaveTimeComponents_ShouldTruncateFractionalDays()
    {
        // 30.5 days between these two timestamps — cast to int truncates to 30
        var startDate = new DateTime(2026, 1, 1, 12, 0, 0);
        var endDate = new DateTime(2026, 2, 1, 0, 0, 0);

        var result = _sut.Compute(startDate, endDate, CoverType.Yacht);

        // Truncated to 30 days (41250), not 31 days (42556.25)
        result.Should().Be(41250m);
    }

    [Fact]
    public void Compute_WhenCoverTypeHasNoStrategy_ShouldThrowPremiumStrategyNotFoundException()
    {
        var unknownCoverType = (CoverType)99;
        var startDate = new DateTime(2026, 1, 1);
        var endDate = startDate.AddDays(30);

        var act = () => _sut.Compute(startDate, endDate, unknownCoverType);

        act.Should().Throw<PremiumStrategyNotFoundException>();
    }
}
