using Claims.Application.Channels;
using Claims.Application.Services;
using Claims.Domain.Enums;
using Claims.Domain.Exceptions;
using Claims.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Claims.Application.Tests.Services;

public class AuditMessageProcessorTests
{
    private readonly Mock<IAuditRepository> _auditRepositoryMock = new();
    private readonly AuditMessageProcessor _sut = new();

    [Fact]
    public async Task ProcessAsync_WhenClaimMessage_ShouldCallAddClaimAudit()
    {
        // Arrange
        var message = new AuditMessage("123", HttpRequestType.POST, AuditEntityType.Claim);

        // Act
        await _sut.ProcessAsync(_auditRepositoryMock.Object, message);

        // Assert
        _auditRepositoryMock.Verify(x => x.AddClaimAuditAsync(
            It.Is<Domain.Models.ClaimAudit>(a => a.ClaimId == "123" && a.HttpRequestType == HttpRequestType.POST),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessAsync_WhenCoverMessage_ShouldCallAddCoverAudit()
    {
        // Arrange
        var message = new AuditMessage("456", HttpRequestType.DELETE, AuditEntityType.Cover);

        // Act
        await _sut.ProcessAsync(_auditRepositoryMock.Object, message);

        // Assert
        _auditRepositoryMock.Verify(x => x.AddCoverAuditAsync(
            It.Is<Domain.Models.CoverAudit>(a => a.CoverId == "456" && a.HttpRequestType == HttpRequestType.DELETE),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessAsync_WhenUnknownEntityType_ShouldThrowUnhandledAuditEntityTypeException()
    {
        // Arrange
        var unknownEntityType = (AuditEntityType)99;
        var message = new AuditMessage("789", HttpRequestType.POST, unknownEntityType);

        // Act
        var act = () => _sut.ProcessAsync(_auditRepositoryMock.Object, message);

        // Assert
        await act.Should().ThrowAsync<UnhandledAuditEntityTypeException>();
    }
}
