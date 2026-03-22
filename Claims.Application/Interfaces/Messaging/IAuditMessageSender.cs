using Claims.Application.Channels;

namespace Claims.Application.Interfaces.Messaging;

public interface IAuditMessageSender
{
    Task SendAsync(AuditMessage message, CancellationToken cancellationToken = default);
}
