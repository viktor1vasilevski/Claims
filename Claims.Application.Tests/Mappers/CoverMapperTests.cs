
namespace Claims.Application.Tests.Mappers;

public class CoverMapperTests
{
    [Fact]
    public void ToDto_ShouldMapAllFields()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 12, 31);
        var cover = Cover.Create(startDate, endDate, CoverType.Yacht, 15_000.50m);

        // Act
        var dto = CoverMapper.ToDto(cover);

        // Assert
        dto.Id.Should().Be(cover.Id);
        dto.StartDate.Should().Be(startDate);
        dto.EndDate.Should().Be(endDate);
        dto.Type.Should().Be(CoverType.Yacht);
        dto.Premium.Should().Be(15_000.50m);
    }

    [Fact]
    public void ToDto_ShouldPreserveCoverType()
    {
        // Arrange
        var cover = Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 6, 1), CoverType.Tanker, 1_000);

        // Act
        var dto = CoverMapper.ToDto(cover);

        // Assert
        dto.Type.Should().Be(CoverType.Tanker);
    }

    [Fact]
    public void ToDto_ShouldPreserveSmallPremiumValue()
    {
        // Arrange
        var cover = Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 6, 1), CoverType.BulkCarrier, 0.01m);

        // Act
        var dto = CoverMapper.ToDto(cover);

        // Assert
        dto.Premium.Should().Be(0.01m);
    }

    [Fact]
    public void ToDto_ShouldPreservePrecisePremiumValue()
    {
        // Arrange
        var cover = Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 6, 1), CoverType.ContainerShip, 12_345.6789m);

        // Act
        var dto = CoverMapper.ToDto(cover);

        // Assert
        dto.Premium.Should().Be(12_345.6789m);
    }
}
