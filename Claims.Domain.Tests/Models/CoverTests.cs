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
        var cover1 = Cover.Create(Start, End, CoverType.Yacht, 0m);
        var cover2 = Cover.Create(Start, End, CoverType.Yacht, 0m);

        cover1.Id.Should().NotBe(cover2.Id);
    }

    [Fact]
    public void Create_WhenEndDateEqualsStartDate_ShouldThrowInvalidCoverPeriodException()
    {
        var act = () => Cover.Create(Start, Start, CoverType.Yacht, 0m);

        act.Should().Throw<InvalidCoverPeriodException>();
    }

    [Fact]
    public void Create_WhenEndDateBeforeStartDate_ShouldThrowInvalidCoverPeriodException()
    {
        var act = () => Cover.Create(End, Start, CoverType.Yacht, 0m);

        act.Should().Throw<InvalidCoverPeriodException>();
    }

    [Fact]
    public void Create_WhenPeriodExceedsOneYear_ShouldThrowInvalidCoverPeriodException()
    {
        var act = () => Cover.Create(Start, Start.AddDays(366), CoverType.Yacht, 0m);

        act.Should().Throw<InvalidCoverPeriodException>();
    }

    [Fact]
    public void Create_WhenPeriodIsExactlyOneYear_ShouldNotThrow()
    {
        var act = () => Cover.Create(Start, Start.AddDays(365), CoverType.Yacht, 0m);

        act.Should().NotThrow();
    }

    [Fact]
    public void IsDateWithinPeriod_WhenDateIsWithinPeriod_ShouldReturnTrue()
    {
        var cover = Cover.Create(Start, End, CoverType.Yacht, 0m);

        cover.IsDateWithinPeriod(new DateTime(2026, 6, 15)).Should().BeTrue();
    }

    [Fact]
    public void IsDateWithinPeriod_WhenDateIsOnStartBoundary_ShouldReturnTrue()
    {
        var cover = Cover.Create(Start, End, CoverType.Yacht, 0m);

        cover.IsDateWithinPeriod(Start).Should().BeTrue();
    }

    [Fact]
    public void IsDateWithinPeriod_WhenDateIsOnEndBoundary_ShouldReturnTrue()
    {
        var cover = Cover.Create(Start, End, CoverType.Yacht, 0m);

        cover.IsDateWithinPeriod(End).Should().BeTrue();
    }

    [Fact]
    public void IsDateWithinPeriod_WhenDateIsBeforeStartDate_ShouldReturnFalse()
    {
        var cover = Cover.Create(Start, End, CoverType.Yacht, 0m);

        cover.IsDateWithinPeriod(Start.AddDays(-1)).Should().BeFalse();
    }

    [Fact]
    public void IsDateWithinPeriod_WhenDateIsAfterEndDate_ShouldReturnFalse()
    {
        var cover = Cover.Create(Start, End, CoverType.Yacht, 0m);

        cover.IsDateWithinPeriod(End.AddDays(1)).Should().BeFalse();
    }
}
