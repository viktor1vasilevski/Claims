
namespace Claims.Application.Tests.Services;

public class ClaimsServiceTests
{
    private static readonly Guid ClaimId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid CoverId1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid CoverId2 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

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
            Claim.Create(CoverId1, "Claim 1", ClaimType.Collision, 1000, DateTime.UtcNow),
            Claim.Create(CoverId2, "Claim 2", ClaimType.Fire, 2000, DateTime.UtcNow)
        };
        _claimsRepositoryMock
            .Setup(x => x.GetClaimsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(claims);

        // Act
        var result = await _sut.GetClaimsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllBeOfType<ClaimDto>();
    }

    [Fact]
    public async Task GetClaimAsync_WhenClaimExists_ShouldReturnClaim()
    {
        // Arrange
        var claim = Claim.Create(CoverId1, "Claim 1", ClaimType.Collision, 1000, DateTime.UtcNow);
        _claimsRepositoryMock
            .Setup(x => x.GetClaimByIdAsync(claim.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(claim);

        // Act
        var result = await _sut.GetClaimByIdAsync(claim.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(claim.Id);
        result.Should().BeOfType<ClaimDto>();
    }

    [Fact]
    public async Task GetClaimAsync_WhenClaimDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _claimsRepositoryMock
            .Setup(x => x.GetClaimByIdAsync(ClaimId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Claim?)null);

        // Act
        var result = await _sut.GetClaimByIdAsync(ClaimId1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateClaimAsync_WhenCoverNotFound_ShouldThrowCoverNotFoundException()
    {
        // Arrange
        var request = new CreateClaimRequest { CoverId = CoverId1, Name = "Test Claim", DamageCost = 5000, Type = ClaimType.Collision, Created = DateTime.UtcNow };
        _coversRepositoryMock
            .Setup(x => x.GetCoverByIdAsync(CoverId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cover?)null);

        // Act
        var act = async () => await _sut.CreateClaimAsync(request);

        // Assert
        await act.Should().ThrowAsync<CoverNotFoundException>()
            .WithMessage($"Cover with id '{CoverId1}' was not found.");
    }

    [Fact]
    public async Task CreateClaimAsync_WhenCreatedDateOutsideCoverPeriod_ShouldThrowClaimDateOutOfRangeException()
    {
        // Arrange
        var cover = Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 12, 31), CoverType.Yacht, 10000m);
        var request = new CreateClaimRequest
        {
            CoverId = cover.Id,
            Created = new DateTime(2025, 1, 1),
            Name = "Test",
            DamageCost = 5000,
            Type = ClaimType.Collision
        };
        _coversRepositoryMock
            .Setup(x => x.GetCoverByIdAsync(cover.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

        // Act
        var act = async () => await _sut.CreateClaimAsync(request);

        // Assert
        await act.Should().ThrowAsync<ClaimDateOutOfRangeException>()
            .WithMessage("Created date must be within the Cover period.");
    }

    [Fact]
    public async Task CreateClaimAsync_WhenCreatedDateAfterCoverEndDate_ShouldThrowClaimDateOutOfRangeException()
    {
        // Arrange
        var cover = Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 12, 31), CoverType.Yacht, 10000m);
        var request = new CreateClaimRequest
        {
            CoverId = cover.Id,
            Created = new DateTime(2027, 1, 1),
            Name = "Test",
            DamageCost = 5000,
            Type = ClaimType.Collision
        };
        _coversRepositoryMock
            .Setup(x => x.GetCoverByIdAsync(cover.Id, It.IsAny<CancellationToken>()))
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
        var cover = Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 12, 31), CoverType.Yacht, 10000m);
        var request = new CreateClaimRequest
        {
            CoverId = cover.Id,
            Created = new DateTime(2026, 6, 1),
            Name = "Test",
            Type = ClaimType.Collision,
            DamageCost = 5000
        };
        _coversRepositoryMock
            .Setup(x => x.GetCoverByIdAsync(cover.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

        // Act
        var result = await _sut.CreateClaimAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ClaimDto>();
        result.CoverId.Should().Be(cover.Id);
        _claimsRepositoryMock.Verify(x => x.CreateClaimAsync(It.IsAny<Claim>(), It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.AuditClaimAsync(It.IsAny<string>(), HttpRequestType.POST), Times.Once);
    }

    [Fact]
    public async Task DeleteClaimAsync_ShouldDeleteClaimAndAudit()
    {
        // Arrange
        var claim = Claim.Create(CoverId1, "Test", ClaimType.Collision, 1000, DateTime.UtcNow);

        _claimsRepositoryMock
            .Setup(x => x.GetClaimByIdAsync(ClaimId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(claim);
        _claimsRepositoryMock
            .Setup(x => x.DeleteClaimAsync(claim, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteClaimAsync(ClaimId1);

        // Assert
        _claimsRepositoryMock.Verify(x => x.DeleteClaimAsync(claim, It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.AuditClaimAsync(ClaimId1.ToString(), HttpRequestType.DELETE), Times.Once);
    }

    [Fact]
    public async Task DeleteClaimAsync_WhenClaimDoesNotExist_ShouldThrowClaimNotFoundException()
    {
        // Arrange
        _claimsRepositoryMock
            .Setup(x => x.GetClaimByIdAsync(ClaimId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Claim?)null);

        // Act
        var act = async () => await _sut.DeleteClaimAsync(ClaimId1);

        // Assert
        await act.Should().ThrowAsync<ClaimNotFoundException>()
            .WithMessage($"Claim with id '{ClaimId1}' was not found.");
        _claimsRepositoryMock.Verify(x => x.DeleteClaimAsync(It.IsAny<Claim>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditServiceMock.Verify(x => x.AuditClaimAsync(It.IsAny<string>(), It.IsAny<HttpRequestType>()), Times.Never);
    }
}
