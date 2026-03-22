using Claims.Application.Channels;

namespace Claims.Application.Interfaces.Messaging;

public interface IAuditMessageReceiver
{
    IAsyncEnumerable<AuditMessage> ReadAllAsync(CancellationToken cancellationToken);
}
