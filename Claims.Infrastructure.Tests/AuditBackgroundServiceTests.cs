using Claims.Application.Channels;
using Claims.Application.Interfaces;
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
    private readonly Mock<IAuditMessageProcessor> _processorMock = new();

    private AuditBackgroundService CreateSut()
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => _auditRepositoryMock.Object);

        var scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();

        return new AuditBackgroundService(_auditChannel, scopeFactory, _processorMock.Object, _loggerMock.Object);
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
        _processorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAuditRepository>(),
            It.Is<AuditMessage>(m => m.Id == "123" && m.HttpRequestType == HttpRequestType.POST && m.EntityType == AuditEntityType.Claim)),
            Times.Once);
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
        _processorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAuditRepository>(),
            It.Is<AuditMessage>(m => m.Id == "456" && m.HttpRequestType == HttpRequestType.DELETE && m.EntityType == AuditEntityType.Cover)),
            Times.Once);
    }
}