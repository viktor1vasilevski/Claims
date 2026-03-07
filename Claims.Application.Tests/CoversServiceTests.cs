using Claims.Application.Interfaces;
using Claims.Application.Requests.Cover;
using Claims.Application.Services;
using Claims.Domain.Enums;
using Claims.Domain.Exceptions;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using FluentAssertions;
using Moq;

namespace Claims.Application.Tests;

public class CoversServiceTests
{
    private readonly Mock<ICoversRepository> _coversRepositoryMock = new();
    private readonly Mock<IAuditService> _auditServiceMock = new();
    private readonly Mock<IPremiumCalculator> _premiumCalculatorMock = new();
    private readonly Mock<IClaimsRepository> _claimRepositoryMock = new();
    private readonly CoversService _sut;

    public CoversServiceTests()
    {
        _sut = new CoversService(
            _coversRepositoryMock.Object,
            _claimRepositoryMock.Object,
            _auditServiceMock.Object,
            _premiumCalculatorMock.Object);

        _premiumCalculatorMock
            .Setup(x => x.ComputeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CoverType>()))
            .ReturnsAsync(50000m);
    }

    [Fact]
    public async Task GetCoversAsync_ShouldReturnAllCovers()
    {
        // Arrange
        var covers = new List<Cover>
        {
            new() { Id = "1", StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 12, 31), Type = CoverType.Yacht },
            new() { Id = "2", StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 12, 31), Type = CoverType.Tanker }
        };
        _coversRepositoryMock.Setup(x => x.GetCoversAsync()).ReturnsAsync(covers);

        // Act
        var result = await _sut.GetCoversAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCoverAsync_WhenCoverExists_ShouldReturnCover()
    {
        // Arrange
        var cover = new Cover { Id = "1", StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 12, 31), Type = CoverType.Yacht };
        _coversRepositoryMock.Setup(x => x.GetCoverAsync("1")).ReturnsAsync(cover);

        // Act
        var result = await _sut.GetCoverAsync("1");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("1");
    }

    [Fact]
    public async Task GetCoverAsync_WhenCoverDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _coversRepositoryMock.Setup(x => x.GetCoverAsync("1")).ReturnsAsync((Cover?)null);

        // Act
        var result = await _sut.GetCoverAsync("1");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateCoverAsync_WhenValid_ShouldCreateCoverAndAudit()
    {
        // Arrange
        var request = new CreateCoverRequest
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            Type = CoverType.Yacht
        };

        // Act
        var result = await _sut.CreateCoverAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(CoverType.Yacht);
        result.Premium.Should().Be(50000m);
        _coversRepositoryMock.Verify(x => x.CreateCoverAsync(It.IsAny<Cover>()), Times.Once);
        _auditServiceMock.Verify(x => x.AuditCoverAsync(It.IsAny<string>(), HttpRequestType.POST), Times.Once);
    }

    [Fact]
    public async Task DeleteCoverAsync_ShouldDeleteCoverAndAudit()
    {
        // Arrange
        _coversRepositoryMock.Setup(x => x.DeleteCoverAsync("1")).Returns(Task.CompletedTask);
        _claimRepositoryMock
            .Setup(x => x.GetClaimsByCoverIdAsync(It.IsAny<string>()))
            .ReturnsAsync(Enumerable.Empty<Claim>());

        // Act
        await _sut.DeleteCoverAsync("1");

        // Assert
        _coversRepositoryMock.Verify(x => x.DeleteCoverAsync("1"), Times.Once);
        _auditServiceMock.Verify(x => x.AuditCoverAsync("1", HttpRequestType.DELETE), Times.Once);
    }
    [Fact]
    public async Task DeleteCoverAsync_WhenCoverHasClaims_ShouldThrowCoverHasActiveClaimsException()
    {
        // Arrange
        var existingClaims = new List<Claim>
        {
            new() { Id = "claim1", CoverId = "1", Name = "Active Claim", DamageCost = 1000, Type = ClaimType.Collision }
        };
        _claimRepositoryMock
            .Setup(x => x.GetClaimsByCoverIdAsync("1"))
            .ReturnsAsync(existingClaims);

        // Act
        var act = async () => await _sut.DeleteCoverAsync("1");

        // Assert
        await act.Should().ThrowAsync<CoverHasActiveClaimsException>();
        _coversRepositoryMock.Verify(x => x.DeleteCoverAsync(It.IsAny<string>()), Times.Never);
        _auditServiceMock.Verify(x => x.AuditCoverAsync(It.IsAny<string>(), It.IsAny<HttpRequestType>()), Times.Never);
    }
}