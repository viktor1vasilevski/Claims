using Claims.Application.Services;
using Claims.Application.Strategies;
using Claims.Domain.Enums;
using FluentAssertions;

namespace Claims.Application.Tests;

public class PremiumCalculatorTests
{
    private readonly PremiumCalculator _sut = new(
    [
        new YachtRatePolicy(),
        new PassengerShipRatePolicy(),
        new TankerRatePolicy(),
        new DefaultRatePolicy(CoverType.ContainerShip),
        new DefaultRatePolicy(CoverType.BulkCarrier)
    ]);

    [Theory]
    [InlineData(CoverType.Yacht, 30, 41250)]
    [InlineData(CoverType.Tanker, 30, 56250)]
    [InlineData(CoverType.PassengerShip, 30, 45000)]
    public async Task ComputeAsync_ShouldReturnCorrectPremium(CoverType coverType, int days, decimal expectedPremium)
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = startDate.AddDays(days);

        // Act
        var result = await _sut.ComputeAsync(startDate, endDate, coverType);

        // Assert
        result.Should().Be(expectedPremium);
    }
}
