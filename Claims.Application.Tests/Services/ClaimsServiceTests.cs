using Claims.Application.Interfaces;
using Claims.Application.Requests.Claims;
using Claims.Application.Services;
using Claims.Domain.Enums;
using Claims.Domain.Exceptions;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using FluentAssertions;
using Moq;

namespace Claims.Application.Tests.Services;

public class ClaimsServiceTests
{
    private readonly Mock<IClaimsRepository> _claimsRepositoryMock = new();
    private readonly Mock<IAuditService> _auditServiceMock = new();
    private readonly Mock<ICoversRepository> _coversRepositoryMock = new();
    private readonly ClaimsService _sut;

    public ClaimsServiceTests()
    {
        _sut = new ClaimsService(
            _claimsRepositoryMock.Object,
            _auditServiceMock.Object,
            _coversRepositoryMock.Object);
    }

    [Fact]
    public async Task GetClaimsAsync_ShouldReturnAllClaims()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new() { Id = "1", CoverId = "c1", Name = "Claim 1", DamageCost = 1000, Type = ClaimType.Collision },
            new() { Id = "2", CoverId = "c2", Name = "Claim 2", DamageCost = 2000, Type = ClaimType.Fire }
        };
        _claimsRepositoryMock
            .Setup(x => x.GetClaimsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(claims);

        // Act
        var result = await _sut.GetClaimsAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetClaimAsync_WhenClaimExists_ShouldReturnClaim()
    {
        // Arrange
        var claim = new Claim { Id = "1", CoverId = "c1", Name = "Claim 1", DamageCost = 1000, Type = ClaimType.Collision };
        _claimsRepositoryMock
            .Setup(x => x.GetClaimByIdAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(claim);

        // Act
        var result = await _sut.GetClaimByIdAsync("1");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("1");
    }

    [Fact]
    public async Task GetClaimAsync_WhenClaimDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _claimsRepositoryMock
            .Setup(x => x.GetClaimByIdAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Claim?)null);

        // Act
        var result = await _sut.GetClaimByIdAsync("1");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateClaimAsync_WhenCoverNotFound_ShouldThrowCoverNotFoundException()
    {
        // Arrange
        var request = new CreateClaimRequest { CoverId = "c1", Name = "Test Claim" };
        _coversRepositoryMock
            .Setup(x => x.GetCoverByIdAsync("c1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cover?)null);

        // Act
        var act = async () => await _sut.CreateClaimAsync(request);

        // Assert
        await act.Should().ThrowAsync<CoverNotFoundException>()
            .WithMessage("Cover with id 'c1' was not found.");
    }

    [Fact]
    public async Task CreateClaimAsync_WhenCreatedDateOutsideCoverPeriod_ShouldThrowClaimDateOutOfRangeException()
    {
        // Arrange
        var cover = new Cover
        {
            Id = "c1",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31)
        };
        var request = new CreateClaimRequest
        {
            CoverId = "c1",
            Created = new DateTime(2025, 1, 1),
            Name = "Test",
        };
        _coversRepositoryMock
            .Setup(x => x.GetCoverByIdAsync("c1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

        // Act
        var act = async () => await _sut.CreateClaimAsync(request);

        // Assert
        await act.Should().ThrowAsync<ClaimDateOutOfRangeException>()
            .WithMessage("Created date must be within the Cover period.");
    }

    [Fact]
    public async Task CreateClaimAsync_WhenValid_ShouldCreateClaimAndAudit()
    {
        // Arrange
        var cover = new Cover
        {
            Id = "c1",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31)
        };
        var request = new CreateClaimRequest
        {
            CoverId = "c1",
            Created = new DateTime(2026, 6, 1),
            Name = "Test",
            Type = ClaimType.Collision,
            DamageCost = 5000
        };
        _coversRepositoryMock
            .Setup(x => x.GetCoverByIdAsync("c1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

        // Act
        var result = await _sut.CreateClaimAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.CoverId.Should().Be("c1");
        _claimsRepositoryMock.Verify(x => x.CreateClaimAsync(It.IsAny<Claim>(), It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.AuditClaimAsync(It.IsAny<string>(), HttpRequestType.POST), Times.Once);
    }

    [Fact]
    public async Task DeleteClaimAsync_ShouldDeleteClaimAndAudit()
    {
        // Arrange
        var claim = new Claim { Id = "1", CoverId = "c1", Name = "Test", DamageCost = 1000, Type = ClaimType.Collision };

        _claimsRepositoryMock
            .Setup(x => x.GetClaimByIdAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(claim);
        _claimsRepositoryMock
            .Setup(x => x.DeleteClaimAsync(claim, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteClaimAsync("1");

        // Assert
        _claimsRepositoryMock.Verify(x => x.DeleteClaimAsync(claim, It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.AuditClaimAsync("1", HttpRequestType.DELETE), Times.Once);
    }

    [Fact]
    public async Task DeleteClaimAsync_WhenClaimDoesNotExist_ShouldThrowClaimNotFoundException()
    {
        // Arrange
        _claimsRepositoryMock
            .Setup(x => x.GetClaimByIdAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Claim?)null);

        // Act
        var act = async () => await _sut.DeleteClaimAsync("1");

        // Assert
        await act.Should().ThrowAsync<ClaimNotFoundException>()
            .WithMessage("Claim with id '1' was not found.");
        _claimsRepositoryMock.Verify(x => x.DeleteClaimAsync(It.IsAny<Claim>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditServiceMock.Verify(x => x.AuditClaimAsync(It.IsAny<string>(), It.IsAny<HttpRequestType>()), Times.Never);
    }
}
