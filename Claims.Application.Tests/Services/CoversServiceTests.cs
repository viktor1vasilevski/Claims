using Claims.Application.Interfaces;
using Claims.Application.Requests.Cover;
using Claims.Application.Services;
using Claims.Domain.Enums;
using Claims.Domain.Exceptions;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using FluentAssertions;
using Moq;

namespace Claims.Application.Tests.Services;

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
        _coversRepositoryMock
            .Setup(x => x.GetCoversAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(covers);

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
        _coversRepositoryMock
            .Setup(x => x.GetCoverAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

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
        _coversRepositoryMock
            .Setup(x => x.GetCoverAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cover?)null);

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
        _coversRepositoryMock.Verify(x => x.CreateCoverAsync(It.IsAny<Cover>(), It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.AuditCoverAsync(It.IsAny<string>(), HttpRequestType.POST), Times.Once);
    }

    [Fact]
    public async Task DeleteCoverAsync_ShouldDeleteCoverAndAudit()
    {
        // Arrange
        var cover = new Cover { Id = "1", StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 12, 31), Type = CoverType.Yacht };

        _coversRepositoryMock
            .Setup(x => x.GetCoverAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);
        _coversRepositoryMock
            .Setup(x => x.DeleteCoverAsync("1", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _claimRepositoryMock
            .Setup(x => x.GetClaimsByCoverIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<Claim>());

        // Act
        await _sut.DeleteCoverAsync("1");

        // Assert
        _coversRepositoryMock.Verify(x => x.DeleteCoverAsync("1", It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.AuditCoverAsync("1", HttpRequestType.DELETE), Times.Once);
    }

    [Fact]
    public async Task DeleteCoverAsync_WhenCoverDoesNotExist_ShouldThrowCoverNotFoundException()
    {
        // Arrange
        _coversRepositoryMock
            .Setup(x => x.GetCoverAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cover?)null);

        // Act
        var act = async () => await _sut.DeleteCoverAsync("1");

        // Assert
        await act.Should().ThrowAsync<CoverNotFoundException>()
            .WithMessage("Cover with id '1' was not found.");
        _coversRepositoryMock.Verify(x => x.DeleteCoverAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditServiceMock.Verify(x => x.AuditCoverAsync(It.IsAny<string>(), It.IsAny<HttpRequestType>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCoverAsync_WhenCoverHasClaims_ShouldThrowCoverHasActiveClaimsException()
    {
        // Arrange
        var cover = new Cover { Id = "1", StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 12, 31), Type = CoverType.Yacht };
        var existingClaims = new List<Claim>
        {
            new() { Id = "claim1", CoverId = "1", Name = "Active Claim", DamageCost = 1000, Type = ClaimType.Collision }
        };

        _coversRepositoryMock
            .Setup(x => x.GetCoverAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);
        _claimRepositoryMock
            .Setup(x => x.GetClaimsByCoverIdAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingClaims);

        // Act
        var act = async () => await _sut.DeleteCoverAsync("1");

        // Assert
        await act.Should().ThrowAsync<CoverHasActiveClaimsException>();
        _coversRepositoryMock.Verify(x => x.DeleteCoverAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditServiceMock.Verify(x => x.AuditCoverAsync(It.IsAny<string>(), It.IsAny<HttpRequestType>()), Times.Never);
    }
}
