using Claims.Application.Channels;
using Claims.Domain.Interfaces;

namespace Claims.Application.Interfaces;

public interface IAuditMessageProcessor
{
    Task ProcessAsync(IAuditRepository auditRepository, AuditMessage message, CancellationToken cancellationToken = default);
}
