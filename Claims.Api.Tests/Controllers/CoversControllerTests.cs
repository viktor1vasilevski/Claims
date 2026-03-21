using Claims.Application.DTOs;
using Claims.Application.Interfaces;
using Claims.Application.Requests.Cover;
using Claims.Controllers;
using Claims.Domain.Enums;
using Claims.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Claims.Api.Tests.Controllers;

public class CoversControllerTests
{
    private readonly Mock<ICoversService> _coversServiceMock = new();
    private readonly CoversController _sut;

    public CoversControllerTests()
    {
        _sut = new CoversController(_coversServiceMock.Object);
    }

    [Fact]
    public async Task Get_ShouldReturn200WithMappedDtos()
    {
        // Arrange
        var covers = new List<Cover>
        {
            Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 6, 1), CoverType.Yacht, 10_000),
            Cover.Create(new DateTime(2026, 2, 1), new DateTime(2026, 7, 1), CoverType.Tanker, 20_000)
        };
        _coversServiceMock
            .Setup(x => x.GetCoversAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(covers);

        // Act
        var result = await _sut.Get(CancellationToken.None);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dtos = ok.Value.Should().BeAssignableTo<IEnumerable<CoverDto>>().Subject;
        dtos.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_WhenCoverExists_ShouldReturn200WithDto()
    {
        // Arrange
        var cover = Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 6, 1), CoverType.Yacht, 10_000);
        _coversServiceMock
            .Setup(x => x.GetCoverByIdAsync(cover.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

        // Act
        var result = await _sut.GetById(cover.Id, CancellationToken.None);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = ok.Value.Should().BeOfType<CoverDto>().Subject;
        dto.Id.Should().Be(cover.Id);
        dto.Type.Should().Be(CoverType.Yacht);
    }

    [Fact]
    public async Task GetById_WhenCoverNotFound_ShouldReturn404()
    {
        // Arrange
        var id = Guid.NewGuid();
        _coversServiceMock
            .Setup(x => x.GetCoverByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cover?)null);

        // Act
        var result = await _sut.GetById(id, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturn201WithDtoAndLocationHeader()
    {
        // Arrange
        var request = new CreateCoverRequest
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 6, 1),
            Type = CoverType.Tanker
        };
        var cover = Cover.Create(new DateTime(2026, 1, 1), new DateTime(2026, 6, 1), CoverType.Tanker, 15_000);
        _coversServiceMock
            .Setup(x => x.CreateCoverAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(_sut.GetById));
        created.RouteValues!["id"].Should().Be(cover.Id);
        var dto = created.Value.Should().BeOfType<CoverDto>().Subject;
        dto.Id.Should().Be(cover.Id);
    }

    [Fact]
    public async Task Delete_ShouldReturn204()
    {
        // Arrange
        var id = Guid.NewGuid();
        _coversServiceMock
            .Setup(x => x.DeleteCoverAsync(id, It.IsAny<CancellationToken>()))
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
        _coversServiceMock
            .Setup(x => x.DeleteCoverAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.Delete(id, CancellationToken.None);

        // Assert
        _coversServiceMock.Verify(x => x.DeleteCoverAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void ComputePremium_ShouldReturn200WithAmount()
    {
        // Arrange
        var request = new ComputePremiumRequest
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 6, 1),
            Type = CoverType.Yacht
        };
        _coversServiceMock
            .Setup(x => x.ComputePremium(request))
            .Returns(12_345.67m);

        // Act
        var result = _sut.ComputePremium(request);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(12_345.67m);
    }
}
