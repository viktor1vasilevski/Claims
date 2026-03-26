namespace Claims.Infrastructure.Tests.BackgroundServices;

public class AuditBackgroundServiceTests
{
    private readonly Mock<IAuditRepository> _auditRepositoryMock = new();
    private readonly Mock<ILogger<AuditBackgroundService>> _loggerMock = new();
    private readonly TestMessageQueue _queue = new();
    private readonly Mock<IAuditMessageProcessor> _processorMock = new();

    private AuditBackgroundService CreateSut()
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => _auditRepositoryMock.Object);

        var scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();

        return new AuditBackgroundService(_queue, scopeFactory, _processorMock.Object, _loggerMock.Object, ResiliencePipeline.Empty);
    }

    [Fact]
    public async Task ExecuteAsync_WhenClaimMessage_ShouldCallAddClaimAudit()
    {
        // Arrange
        var sut = CreateSut();
        var cts = new CancellationTokenSource();

        // Act
        var task = sut.StartAsync(cts.Token);
        await _queue.SendAsync(new AuditMessage("123", HttpRequestType.POST, AuditEntityType.Claim));
        await Task.Delay(100);
        await cts.CancelAsync();

        // Assert
        _processorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAuditRepository>(),
            It.Is<AuditMessage>(m => m.Id == "123" && m.HttpRequestType == HttpRequestType.POST && m.EntityType == AuditEntityType.Claim),
            It.IsAny<CancellationToken>()),
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
        await _queue.SendAsync(new AuditMessage("456", HttpRequestType.DELETE, AuditEntityType.Cover));
        await Task.Delay(100);
        await cts.CancelAsync();

        // Assert
        _processorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAuditRepository>(),
            It.Is<AuditMessage>(m => m.Id == "456" && m.HttpRequestType == HttpRequestType.DELETE && m.EntityType == AuditEntityType.Cover),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProcessorThrows_ShouldLogErrorAndContinue()
    {
        // Arrange
        var sut = CreateSut();
        var cts = new CancellationTokenSource();
        var processedCount = 0;
        var allProcessed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        _processorMock
            .Setup(x => x.ProcessAsync(It.IsAny<IAuditRepository>(), It.IsAny<AuditMessage>(), It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                if (Interlocked.Increment(ref processedCount) == 2)
                    allProcessed.TrySetResult();
            })
            .ThrowsAsync(new Exception("Processing failed"));

        // Act
        _ = sut.StartAsync(cts.Token);
        await _queue.SendAsync(new AuditMessage("123", HttpRequestType.POST, AuditEntityType.Claim));
        await _queue.SendAsync(new AuditMessage("456", HttpRequestType.POST, AuditEntityType.Claim));

        // Wait until both messages have been attempted, with a timeout to prevent the test hanging
        await allProcessed.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await cts.CancelAsync();

        // Assert - both messages were attempted despite the first throwing
        _processorMock.Verify(x => x.ProcessAsync(
            It.IsAny<IAuditRepository>(),
            It.IsAny<AuditMessage>(),
            It.IsAny<CancellationToken>()),
            Times.Exactly(2));

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task ExecuteAsync_WhenMongoWriteSucceeds_ShouldAcknowledgeMessage()
    {
        // Arrange
        var sut = CreateSut();
        var cts = new CancellationTokenSource();
        var acknowledged = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        // Act
        _ = sut.StartAsync(cts.Token);
        await _queue.SendAsync(
            new AuditMessage("123", HttpRequestType.POST, AuditEntityType.Claim),
            () => { acknowledged.TrySetResult(); return Task.CompletedTask; });

        await acknowledged.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await cts.CancelAsync();

        // Assert - message was acknowledged, so it would be removed from the queue
        acknowledged.Task.IsCompletedSuccessfully.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WhenMongoWriteFails_ShouldNotAcknowledgeMessage()
    {
        // Arrange
        var sut = CreateSut();
        var cts = new CancellationTokenSource();
        var acknowledged = false;
        var processed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        _processorMock
            .Setup(x => x.ProcessAsync(It.IsAny<IAuditRepository>(), It.IsAny<AuditMessage>(), It.IsAny<CancellationToken>()))
            .Callback(() => processed.TrySetResult())
            .ThrowsAsync(new Exception("MongoDB is down"));

        // Act
        _ = sut.StartAsync(cts.Token);
        await _queue.SendAsync(
            new AuditMessage("123", HttpRequestType.POST, AuditEntityType.Claim),
            () => { acknowledged = true; return Task.CompletedTask; });

        await processed.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await cts.CancelAsync();

        // Assert - message was not acknowledged, so Service Bus will redeliver it
        acknowledged.Should().BeFalse();
    }

    private sealed class TestMessageQueue : IAuditMessageSender, IAuditMessageReceiver
    {
        private readonly Channel<AuditMessageEnvelope> _channel = Channel.CreateUnbounded<AuditMessageEnvelope>();

        public async Task SendAsync(AuditMessage message, CancellationToken cancellationToken = default)
            => await _channel.Writer.WriteAsync(new AuditMessageEnvelope(message, () => Task.CompletedTask), cancellationToken);

        public async Task SendAsync(AuditMessage message, Func<Task> acknowledgeAsync, CancellationToken cancellationToken = default)
            => await _channel.Writer.WriteAsync(new AuditMessageEnvelope(message, acknowledgeAsync), cancellationToken);

        public IAsyncEnumerable<AuditMessageEnvelope> ReadAllAsync(CancellationToken cancellationToken)
            => _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
