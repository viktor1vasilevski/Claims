using Claims.Application.Mappers;
using Claims.Domain.Enums;
using Claims.Domain.Models;
using FluentAssertions;

namespace Claims.Application.Tests.Mappers;

public class ClaimMapperTests
{
    [Fact]
    public void ToDto_ShouldMapAllFields()
    {
        // Arrange
        var coverId = Guid.NewGuid();
        var created = new DateTime(2026, 6, 15, 10, 30, 0, DateTimeKind.Utc);
        var claim = Claim.Create(coverId, "Hull Damage", ClaimType.Collision, 50_000, created);

        // Act
        var dto = ClaimMapper.ToDto(claim);

        // Assert
        dto.Id.Should().Be(claim.Id);
        dto.CoverId.Should().Be(coverId);
        dto.Created.Should().Be(created);
        dto.Name.Should().Be("Hull Damage");
        dto.Type.Should().Be(ClaimType.Collision);
        dto.DamageCost.Should().Be(50_000);
    }

    [Fact]
    public void ToDto_ShouldPreserveClaimType()
    {
        // Arrange
        var claim = Claim.Create(Guid.NewGuid(), "Fire Claim", ClaimType.Fire, 1_000, DateTime.UtcNow);

        // Act
        var dto = ClaimMapper.ToDto(claim);

        // Assert
        dto.Type.Should().Be(ClaimType.Fire);
    }

    [Fact]
    public void ToDto_ShouldPreserveMinimumDamageCost()
    {
        // Arrange
        var claim = Claim.Create(Guid.NewGuid(), "Minor Damage", ClaimType.BadWeather, 1, DateTime.UtcNow);

        // Act
        var dto = ClaimMapper.ToDto(claim);

        // Assert
        dto.DamageCost.Should().Be(1);
    }

    [Fact]
    public void ToDto_ShouldPreserveMaximumDamageCost()
    {
        // Arrange
        var claim = Claim.Create(Guid.NewGuid(), "Total Loss", ClaimType.Grounding, 100_000, DateTime.UtcNow);

        // Act
        var dto = ClaimMapper.ToDto(claim);

        // Assert
        dto.DamageCost.Should().Be(100_000);
    }
}
