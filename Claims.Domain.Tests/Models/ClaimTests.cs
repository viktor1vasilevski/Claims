namespace Claims.Domain.Tests.Models;

public class ClaimTests
{
    private static readonly Guid ValidCoverId = Guid.NewGuid();

    [Fact]
    public void Create_WhenValid_ShouldReturnClaimWithCorrectProperties()
    {
        var created = new DateTime(2026, 6, 1);

        var claim = Claim.Create(ValidCoverId, "Hull damage", ClaimType.Collision, 5000m, created);

        claim.Id.Should().NotBe(Guid.Empty);
        claim.CoverId.Should().Be(ValidCoverId);
        claim.Name.Should().Be("Hull damage");
        claim.Type.Should().Be(ClaimType.Collision);
        claim.DamageCost.Should().Be(5000m);
        claim.Created.Should().Be(created);
    }

    [Fact]
    public void Create_ShouldGenerateUniqueIds()
    {
        var claim1 = Claim.Create(ValidCoverId, "Claim 1", ClaimType.Fire, 1000m, DateTime.UtcNow);
        var claim2 = Claim.Create(ValidCoverId, "Claim 2", ClaimType.Fire, 1000m, DateTime.UtcNow);

        claim1.Id.Should().NotBe(claim2.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_WhenDamageCostIsZeroOrNegative_ShouldThrowInvalidDamageCostException(decimal damageCost)
    {
        var act = () => Claim.Create(ValidCoverId, "Test", ClaimType.Collision, damageCost, DateTime.UtcNow);

        act.Should().Throw<InvalidDamageCostException>();
    }

    [Theory]
    [InlineData(100_001)]
    [InlineData(200_000)]
    public void Create_WhenDamageCostExceedsLimit_ShouldThrowInvalidDamageCostException(decimal damageCost)
    {
        var act = () => Claim.Create(ValidCoverId, "Test", ClaimType.Collision, damageCost, DateTime.UtcNow);

        act.Should().Throw<InvalidDamageCostException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50_000)]
    [InlineData(100_000)]
    public void Create_WhenDamageCostIsWithinBounds_ShouldNotThrow(decimal damageCost)
    {
        var act = () => Claim.Create(ValidCoverId, "Test", ClaimType.Collision, damageCost, DateTime.UtcNow);

        act.Should().NotThrow();
    }
}
