namespace Claims.Infrastructure.Tests.Messaging;

public class ServiceBusAuditQueueTests
{
    private readonly Mock<ServiceBusClient> _clientMock = new();
    private readonly Mock<ServiceBusSender> _senderMock = new();
    private readonly Mock<ILogger<ServiceBusAuditQueue>> _loggerMock = new();

    private ServiceBusAuditQueue CreateSut()
    {
        _clientMock
            .Setup(c => c.CreateSender(It.IsAny<string>()))
            .Returns(_senderMock.Object);

        return new ServiceBusAuditQueue(_clientMock.Object, "audit-queue", _loggerMock.Object);
    }

    [Fact]
    public async Task SendAsync_ShouldSendSerializedMessageToServiceBus()
    {
        // Arrange
        var sut = CreateSut();
        var message = new AuditMessage("claim-123", HttpRequestType.POST, AuditEntityType.Claim);
        ServiceBusMessage? capturedMessage = null;

        _senderMock
            .Setup(s => s.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
            .Callback<ServiceBusMessage, CancellationToken>((m, _) => capturedMessage = m)
            .Returns(Task.CompletedTask);

        // Act
        await sut.SendAsync(message);

        // Assert
        _senderMock.Verify(s => s.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);

        capturedMessage.Should().NotBeNull();
        var body = capturedMessage!.Body.ToString();
        body.Should().Contain("claim-123");
        body.Should().Contain("POST");   // enum serialized as string, not number
        body.Should().Contain("Claim");  // enum serialized as string, not number
    }

    [Fact]
    public async Task SendAsync_WhenServiceBusThrows_ShouldLogWarningAndNotRethrow()
    {
        // Arrange
        var sut = CreateSut();
        var message = new AuditMessage("claim-123", HttpRequestType.POST, AuditEntityType.Claim);

        _senderMock
            .Setup(s => s.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ServiceBusException("Connection failed", ServiceBusFailureReason.ServiceCommunicationProblem));

        // Act
        var act = async () => await sut.SendAsync(message);

        // Assert - exception must not bubble up
        await act.Should().NotThrowAsync();

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_ShouldSerializeEnumsAsStrings()
    {
        // Arrange
        var sut = CreateSut();
        var message = new AuditMessage("cover-456", HttpRequestType.DELETE, AuditEntityType.Cover);
        ServiceBusMessage? capturedMessage = null;

        _senderMock
            .Setup(s => s.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
            .Callback<ServiceBusMessage, CancellationToken>((m, _) => capturedMessage = m)
            .Returns(Task.CompletedTask);

        // Act
        await sut.SendAsync(message);

        // Assert - enums must be strings not integers
        var body = capturedMessage!.Body.ToString();
        var json = JsonDocument.Parse(body);

        json.RootElement.GetProperty("HttpRequestType").GetString().Should().Be("DELETE");
        json.RootElement.GetProperty("EntityType").GetString().Should().Be("Cover");
    }
}
