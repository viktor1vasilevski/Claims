using Claims.Application.Channels;

namespace Claims.Application.Interfaces;

public interface IAuditMessageProcessor
{
    Task ProcessAsync(IAuditRepository auditRepository, AuditMessage message, CancellationToken cancellationToken = default);
}
