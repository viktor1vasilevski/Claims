using Claims.Application.Requests.Claims;
using Claims.Controllers;

namespace Claims.Api.Tests.Controllers;

public class ClaimsControllerTests
{
    private readonly Mock<IClaimsService> _claimsServiceMock = new();
    private readonly ClaimsController _sut;

    public ClaimsControllerTests()
    {
        _sut = new ClaimsController(_claimsServiceMock.Object);
    }

    [Fact]
    public async Task Get_ShouldReturn200WithDtos()
    {
        // Arrange
        var dtos = new List<ClaimDto>
        {
            new() { Id = Guid.NewGuid(), CoverId = Guid.NewGuid(), Name = "Claim 1", Type = ClaimType.Collision, DamageCost = 1000 },
            new() { Id = Guid.NewGuid(), CoverId = Guid.NewGuid(), Name = "Claim 2", Type = ClaimType.Fire, DamageCost = 2000 }
        };
        _claimsServiceMock
            .Setup(x => x.GetClaimsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dtos);

        // Act
        var result = await _sut.Get(CancellationToken.None);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = ok.Value.Should().BeAssignableTo<IEnumerable<ClaimDto>>().Subject;
        returned.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_WhenClaimExists_ShouldReturn200WithDto()
    {
        // Arrange
        var dto = new ClaimDto { Id = Guid.NewGuid(), CoverId = Guid.NewGuid(), Name = "Test Claim", Type = ClaimType.Collision, DamageCost = 5000 };
        _claimsServiceMock
            .Setup(x => x.GetClaimByIdAsync(dto.Id!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _sut.GetById(dto.Id!.Value, CancellationToken.None);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = ok.Value.Should().BeOfType<ClaimDto>().Subject;
        returned.Id.Should().Be(dto.Id);
        returned.Name.Should().Be("Test Claim");
    }

    [Fact]
    public async Task GetById_WhenClaimNotFound_ShouldReturn404()
    {
        // Arrange
        var id = Guid.NewGuid();
        _claimsServiceMock
            .Setup(x => x.GetClaimByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClaimDto?)null);

        // Act
        var result = await _sut.GetById(id, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturn201WithDtoAndLocationHeader()
    {
        // Arrange
        var coverId = Guid.NewGuid();
        var request = new CreateClaimRequest
        {
            CoverId = coverId,
            Name = "New Claim",
            Type = ClaimType.Fire,
            DamageCost = 10_000,
            Created = DateTime.UtcNow
        };
        var dto = new ClaimDto { Id = Guid.NewGuid(), CoverId = coverId, Name = "New Claim", Type = ClaimType.Fire, DamageCost = 10_000 };
        _claimsServiceMock
            .Setup(x => x.CreateClaimAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(_sut.GetById));
        created.RouteValues!["id"].Should().Be(dto.Id);
        var returned = created.Value.Should().BeOfType<ClaimDto>().Subject;
        returned.Id.Should().Be(dto.Id);
    }

    [Fact]
    public async Task Delete_ShouldReturn204()
    {
        // Arrange
        var id = Guid.NewGuid();
        _claimsServiceMock
            .Setup(x => x.DeleteClaimAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Delete(id, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ShouldCallServiceWithCorrectId()
    {
        // Arrange
        var id = Guid.NewGuid();
        _claimsServiceMock
            .Setup(x => x.DeleteClaimAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.Delete(id, CancellationToken.None);

        // Assert
        _claimsServiceMock.Verify(x => x.DeleteClaimAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
