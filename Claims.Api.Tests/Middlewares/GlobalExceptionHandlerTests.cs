using Claims.Api.Middlewares;
using Claims.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Claims.Api.Tests.Middlewares;

public class GlobalExceptionHandlerTests
{
    private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock = new();
    private readonly Mock<IProblemDetailsService> _problemDetailsServiceMock = new();
    private readonly GlobalExceptionHandler _sut;

    public GlobalExceptionHandlerTests()
    {
        _problemDetailsServiceMock
            .Setup(x => x.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(true);

        _sut = new GlobalExceptionHandler(_loggerMock.Object, _problemDetailsServiceMock.Object);
    }

    [Fact]
    public async Task TryHandleAsync_WhenCoverNotFoundException_ShouldReturn404()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var exception = new CoverNotFoundException(Guid.NewGuid());

        // Act
        var result = await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task TryHandleAsync_WhenClaimNotFoundException_ShouldReturn404()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var exception = new ClaimNotFoundException(Guid.NewGuid());

        // Act
        var result = await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task TryHandleAsync_WhenClaimDateOutOfRangeException_ShouldReturn400()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var exception = new ClaimDateOutOfRangeException();

        // Act
        var result = await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task TryHandleAsync_WhenPremiumStrategyNotFoundException_ShouldReturn400()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var exception = new PremiumStrategyNotFoundException(CoverType.Yacht);

        // Act
        var result = await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task TryHandleAsync_WhenCoverHasActiveClaimsException_ShouldReturn409()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var exception = new CoverHasActiveClaimsException(Guid.NewGuid());

        // Act
        var result = await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task TryHandleAsync_WhenUnhandledAuditEntityTypeException_ShouldReturn500()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var exception = new UnhandledAuditEntityTypeException(AuditEntityType.Claim);

        // Act
        var result = await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task TryHandleAsync_WhenUnknownException_ShouldReturn500()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var exception = new InvalidOperationException("something went wrong");

        // Act
        var result = await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task TryHandleAsync_WhenUnknownException_ShouldUseGenericMessage()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var exception = new InvalidOperationException("internal detail");

        // Act
        await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        _problemDetailsServiceMock.Verify(x => x.TryWriteAsync(
            It.Is<ProblemDetailsContext>(ctx =>
                ctx.ProblemDetails.Detail == "An unexpected error occurred.")),
            Times.Once);
    }

    [Fact]
    public async Task TryHandleAsync_WhenDomainException_ShouldUseExceptionMessage()
    {
        // Arrange
        var coverId = Guid.NewGuid();
        var httpContext = new DefaultHttpContext();
        var exception = new CoverNotFoundException(coverId);

        // Act
        await _sut.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        _problemDetailsServiceMock.Verify(x => x.TryWriteAsync(
            It.Is<ProblemDetailsContext>(ctx =>
                ctx.ProblemDetails.Detail == exception.Message)),
            Times.Once);
    }
}
