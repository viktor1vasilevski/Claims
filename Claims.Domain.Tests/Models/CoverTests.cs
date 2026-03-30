namespace Claims.Domain.Tests.Models;

public class CoverTests
{
    private static readonly DateTime Start = new(2026, 1, 1);
    private static readonly DateTime End = new(2026, 12, 31);

    [Fact]
    public void Create_WhenValid_ShouldReturnCoverWithCorrectProperties()
    {
        var cover = Cover.Create(Start, End, CoverType.Yacht, 50000m);

        cover.Id.Should().NotBe(Guid.Empty);
        cover.StartDate.Should().Be(Start);
        cover.EndDate.Should().Be(End);
        cover.Type.Should().Be(CoverType.Yacht);
        cover.Premium.Should().Be(50000m);
    }

    [Fact]
    public void Create_ShouldGenerateUniqueIds()
    {
        var cover1 = Cover.Create(Start, End, CoverType.Yacht, 1000m);
        var cover2 = Cover.Create(Start, End, CoverType.Yacht, 1000m);

        cover1.Id.Should().NotBe(cover2.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_WhenPremiumIsZeroOrNegative_ShouldThrowInvalidPremiumException(decimal premium)
    {
        var act = () => Cover.Create(Start, End, CoverType.Yacht, premium);

        act.Should().Throw<InvalidPremiumException>();
    }

    [Fact]
    public void Create_WhenEndDateEqualsStartDate_ShouldThrowInvalidCoverPeriodException()
    {
        var act = () => Cover.Create(Start, Start, CoverType.Yacht, 1000m);

        act.Should().Throw<InvalidCoverPeriodException>();
    }

    [Fact]
    public void Create_WhenEndDateBeforeStartDate_ShouldThrowInvalidCoverPeriodException()
    {
        var act = () => Cover.Create(End, Start, CoverType.Yacht, 1000m);

        act.Should().Throw<InvalidCoverPeriodException>();
    }

    [Fact]
    public void Create_WhenPeriodExceedsOneYear_ShouldThrowInvalidCoverPeriodException()
    {
        var act = () => Cover.Create(Start, Start.AddDays(366), CoverType.Yacht, 1000m);

        act.Should().Throw<InvalidCoverPeriodException>();
    }

    [Fact]
    public void Create_WhenPeriodIsExactlyOneYear_ShouldNotThrow()
    {
        var act = () => Cover.Create(Start, Start.AddDays(365), CoverType.Yacht, 1000m);

        act.Should().NotThrow();
    }

    [Fact]
    public void Create_WhenPeriodIsLessThanOneDay_ShouldNotThrow()
    {
        // The domain accepts sub-day periods as long as premium > 0.
        // In practice the service computes the premium via the calculator first;
        // the calculator truncates fractional days to zero, so Cover.Create ends up
        // receiving 0m and throws InvalidPremiumException — an indirect and misleading error.
        var start = new DateTime(2026, 1, 1, 0, 0, 0);
        var end = new DateTime(2026, 1, 1, 12, 0, 0);

        var act = () => Cover.Create(start, end, CoverType.Yacht, 1000m);

        act.Should().NotThrow();
    }

    [Fact]
    public void IsDateWithinPeriod_WhenDateIsWithinPeriod_ShouldReturnTrue()
    {
        var cover = Cover.Create(Start, End, CoverType.Yacht, 1000m);

        cover.IsDateWithinPeriod(new DateTime(2026, 6, 15)).Should().BeTrue();
    }

    [Fact]
    public void IsDateWithinPeriod_WhenDateIsOnStartBoundary_ShouldReturnTrue()
    {
        var cover = Cover.Create(Start, End, CoverType.Yacht, 1000m);

        cover.IsDateWithinPeriod(Start).Should().BeTrue();
    }

    [Fact]
    public void IsDateWithinPeriod_WhenDateIsOnEndBoundary_ShouldReturnTrue()
    {
        var cover = Cover.Create(Start, End, CoverType.Yacht, 1000m);

        cover.IsDateWithinPeriod(End).Should().BeTrue();
    }

    [Fact]
    public void IsDateWithinPeriod_WhenDateIsBeforeStartDate_ShouldReturnFalse()
    {
        var cover = Cover.Create(Start, End, CoverType.Yacht, 1000m);

        cover.IsDateWithinPeriod(Start.AddDays(-1)).Should().BeFalse();
    }

    [Fact]
    public void IsDateWithinPeriod_WhenDateIsAfterEndDate_ShouldReturnFalse()
    {
        var cover = Cover.Create(Start, End, CoverType.Yacht, 1000m);

        cover.IsDateWithinPeriod(End.AddDays(1)).Should().BeFalse();
    }
}
