using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Application.Requests.Claims;
using Claims.Controllers;
using Claims.Domain.Enums;
using Claims.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
    public async Task Get_ShouldReturn200WithMappedDtos()
    {
        // Arrange
        var coverId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            Claim.Create(coverId, "Claim 1", ClaimType.Collision, 1000, DateTime.UtcNow),
            Claim.Create(coverId, "Claim 2", ClaimType.Fire, 2000, DateTime.UtcNow)
        };
        _claimsServiceMock
            .Setup(x => x.GetClaimsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(claims);

        // Act
        var result = await _sut.Get(CancellationToken.None);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dtos = ok.Value.Should().BeAssignableTo<IEnumerable<ClaimDto>>().Subject;
        dtos.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_WhenClaimExists_ShouldReturn200WithDto()
    {
        // Arrange
        var claim = Claim.Create(Guid.NewGuid(), "Test Claim", ClaimType.Collision, 5000, DateTime.UtcNow);
        _claimsServiceMock
            .Setup(x => x.GetClaimByIdAsync(claim.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(claim);

        // Act
        var result = await _sut.GetById(claim.Id, CancellationToken.None);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = ok.Value.Should().BeOfType<ClaimDto>().Subject;
        dto.Id.Should().Be(claim.Id);
        dto.Name.Should().Be("Test Claim");
    }

    [Fact]
    public async Task GetById_WhenClaimNotFound_ShouldReturn404()
    {
        // Arrange
        var id = Guid.NewGuid();
        _claimsServiceMock
            .Setup(x => x.GetClaimByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Claim?)null);

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
        var claim = Claim.Create(coverId, "New Claim", ClaimType.Fire, 10_000, DateTime.UtcNow);
        _claimsServiceMock
            .Setup(x => x.CreateClaimAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(claim);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(_sut.GetById));
        created.RouteValues!["id"].Should().Be(claim.Id);
        var dto = created.Value.Should().BeOfType<ClaimDto>().Subject;
        dto.Id.Should().Be(claim.Id);
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
