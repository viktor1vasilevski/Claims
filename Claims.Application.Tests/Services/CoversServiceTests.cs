
namespace Claims.Application.Tests.Services;

public class CoversServiceTests
{
    private static readonly Guid CoverId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid CoverId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

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
            .Setup(x => x.Compute(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CoverType>()))
            .Returns(50000m);
    }

    [Fact]
    public async Task GetCoversAsync_WhenNoCoversExist_ShouldReturnEmptyCollection()
    {
        _coversRepositoryMock
            .Setup(x => x.GetCoversAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Cover>());

        var result = await _sut.GetCoversAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCoversAsync_ShouldReturnAllCovers()
    {
        // Arrange
        var covers = new List<Cover>
        {
            Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 12, 31), CoverType.Yacht, 10000m),
            Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 12, 31), CoverType.Tanker, 20000m)
        };
        _coversRepositoryMock
            .Setup(x => x.GetCoversAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(covers);

        // Act
        var result = await _sut.GetCoversAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllBeOfType<CoverDto>();
    }

    [Fact]
    public async Task GetCoverAsync_WhenCoverExists_ShouldReturnCover()
    {
        // Arrange
        var cover = Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 12, 31), CoverType.Yacht, 10000m);
        _coversRepositoryMock
            .Setup(x => x.GetCoverByIdAsync(cover.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

        // Act
        var result = await _sut.GetCoverByIdAsync(cover.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CoverDto>();
        result!.Id.Should().Be(cover.Id);
    }

    [Fact]
    public async Task GetCoverAsync_WhenCoverDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _coversRepositoryMock
            .Setup(x => x.GetCoverByIdAsync(CoverId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cover?)null);

        // Act
        var result = await _sut.GetCoverByIdAsync(CoverId1);

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
        result.Should().BeOfType<CoverDto>();
        result.Type.Should().Be(CoverType.Yacht);
        result.Premium.Should().Be(50000m);
        _coversRepositoryMock.Verify(x => x.CreateCoverAsync(It.IsAny<Cover>(), It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.AuditCoverAsync(It.IsAny<string>(), HttpRequestType.POST), Times.Once);
    }

    [Fact]
    public async Task DeleteCoverAsync_ShouldDeleteCoverAndAudit()
    {
        // Arrange
        var cover = Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 12, 31), CoverType.Yacht, 10000m);

        _coversRepositoryMock
            .Setup(x => x.GetCoverByIdAsync(CoverId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);
        _coversRepositoryMock
            .Setup(x => x.DeleteCoverAsync(cover, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _claimRepositoryMock
            .Setup(x => x.GetClaimsByCoverIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Claim>());

        // Act
        await _sut.DeleteCoverAsync(CoverId1);

        // Assert
        _coversRepositoryMock.Verify(x => x.DeleteCoverAsync(cover, It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(x => x.AuditCoverAsync(CoverId1.ToString(), HttpRequestType.DELETE), Times.Once);
    }

    [Fact]
    public async Task DeleteCoverAsync_WhenCoverDoesNotExist_ShouldThrowCoverNotFoundException()
    {
        // Arrange
        _coversRepositoryMock
            .Setup(x => x.GetCoverByIdAsync(CoverId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cover?)null);

        // Act
        var act = async () => await _sut.DeleteCoverAsync(CoverId1);

        // Assert
        await act.Should().ThrowAsync<CoverNotFoundException>()
            .WithMessage($"Cover with id '{CoverId1}' was not found.");
        _coversRepositoryMock.Verify(x => x.DeleteCoverAsync(It.IsAny<Cover>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditServiceMock.Verify(x => x.AuditCoverAsync(It.IsAny<string>(), It.IsAny<HttpRequestType>()), Times.Never);
    }

    [Fact]
    public async Task CreateCoverAsync_WhenCalculatorReturnsZeroPremium_ShouldThrowInvalidPremiumException()
    {
        // When a sub-day period is requested, the calculator truncates fractional days
        // to zero and returns 0m. Cover.Create then throws InvalidPremiumException —
        // a misleading error that says nothing about the period being too short.
        var request = new CreateCoverRequest
        {
            StartDate = new DateTime(2026, 1, 1, 0, 0, 0),
            EndDate = new DateTime(2026, 1, 1, 12, 0, 0),
            Type = CoverType.Yacht
        };
        _premiumCalculatorMock
            .Setup(x => x.Compute(request.StartDate, request.EndDate, request.Type))
            .Returns(0m);

        var act = async () => await _sut.CreateCoverAsync(request);

        await act.Should().ThrowAsync<InvalidPremiumException>();
    }

    [Fact]
    public async Task DeleteCoverAsync_WhenCoverHasClaims_ShouldThrowCoverHasActiveClaimsException()
    {
        // Arrange
        var cover = Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 12, 31), CoverType.Yacht, 10000m);
        var existingClaims = new List<Claim>
        {
            Claim.Create(CoverId1, "Active Claim", ClaimType.Collision, 1000, DateTime.UtcNow)
        };

        _coversRepositoryMock
            .Setup(x => x.GetCoverByIdAsync(CoverId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);
        _claimRepositoryMock
            .Setup(x => x.GetClaimsByCoverIdAsync(CoverId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingClaims);

        // Act
        var act = async () => await _sut.DeleteCoverAsync(CoverId1);

        // Assert
        await act.Should().ThrowAsync<CoverHasActiveClaimsException>();
        _coversRepositoryMock.Verify(x => x.DeleteCoverAsync(It.IsAny<Cover>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditServiceMock.Verify(x => x.AuditCoverAsync(It.IsAny<string>(), It.IsAny<HttpRequestType>()), Times.Never);
    }
}
