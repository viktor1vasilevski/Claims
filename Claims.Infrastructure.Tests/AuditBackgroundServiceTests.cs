using Claims.Application.Channels;
using Claims.Domain.Enums;
using Claims.Domain.Interfaces;
using Claims.Domain.Models;
using Claims.Infrastructure.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Claims.Infrastructure.Tests;

public class AuditBackgroundServiceTests
{
    private readonly Mock<IAuditRepository> _auditRepositoryMock = new();
    private readonly Mock<ILogger<AuditBackgroundService>> _loggerMock = new();
    private readonly AuditChannel _auditChannel = new();

    private AuditBackgroundService CreateSut()
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => _auditRepositoryMock.Object);

        var scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();

        return new AuditBackgroundService(_auditChannel, scopeFactory, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenClaimMessage_ShouldCallAddClaimAudit()
    {
        // Arrange
        var sut = CreateSut();
        var cts = new CancellationTokenSource();

        // Act
        var task = sut.StartAsync(cts.Token);
        await _auditChannel.Writer.WriteAsync(new AuditMessage("123", HttpRequestType.POST, AuditEntityType.Claim));
        await Task.Delay(100);
        await cts.CancelAsync();

        // Assert
        _auditRepositoryMock.Verify(x => x.AddClaimAuditAsync(It.Is<ClaimAudit>(
            a => a.ClaimId == "123" && a.HttpRequestType == HttpRequestType.POST)), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCoverMessage_ShouldCallAddCoverAudit()
    {
        // Arrange
        var sut = CreateSut();
        var cts = new CancellationTokenSource();

        // Act
        var task = sut.StartAsync(cts.Token);
        await _auditChannel.Writer.WriteAsync(new AuditMessage("456", HttpRequestType.DELETE, AuditEntityType.Cover));
        await Task.Delay(100);
        await cts.CancelAsync();

        // Assert
        _auditRepositoryMock.Verify(x => x.AddCoverAuditAsync(It.Is<CoverAudit>(
            a => a.CoverId == "456" && a.HttpRequestType == HttpRequestType.DELETE)), Times.Once);
    }
}