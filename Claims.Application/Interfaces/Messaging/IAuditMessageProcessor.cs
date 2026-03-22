using Claims.Application.Channels;

namespace Claims.Application.Interfaces.Messaging;

public interface IAuditMessageProcessor
{
    Task ProcessAsync(IAuditRepository auditRepository, AuditMessage message, CancellationToken cancellationToken = default);
}
