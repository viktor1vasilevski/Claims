using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Application.Strategies;
using Claims.Domain.Enums;
using FluentAssertions;

namespace Claims.Application.Tests;

public class PremiumCalculatorTests
{
    private readonly PremiumCalculator _sut = new(new IPremiumRateStrategy[]
    {
        new YachtRateStrategy(),
        new TankerRateStrategy(),
        new PassengerShipRateStrategy(),
        new ContainerShipRateStrategy(),
        new BulkCarrierRateStrategy()
    });

    [Theory]
    // First period only - no discounts
    [InlineData(CoverType.Yacht, 30, 41250)]
    [InlineData(CoverType.PassengerShip, 30, 45000)]
    [InlineData(CoverType.ContainerShip, 30, 48750)]
    [InlineData(CoverType.BulkCarrier, 30, 48750)]
    [InlineData(CoverType.Tanker, 30, 56250)]
    // Cross into second period - first discount applies
    [InlineData(CoverType.Yacht, 31, 42556.25)]
    [InlineData(CoverType.PassengerShip, 31, 46470)]
    [InlineData(CoverType.ContainerShip, 31, 50342.5)]
    [InlineData(CoverType.BulkCarrier, 31, 50342.5)]
    [InlineData(CoverType.Tanker, 31, 58087.5)]
    public async Task ComputeAsync_ShouldReturnCorrectPremium(CoverType coverType, int days, decimal expectedPremium)
    {
        var startDate = new DateTime(2026, 1, 1);
        var endDate = startDate.AddDays(days);

        var result = await _sut.ComputeAsync(startDate, endDate, coverType);

        result.Should().Be(expectedPremium);
    }
}
