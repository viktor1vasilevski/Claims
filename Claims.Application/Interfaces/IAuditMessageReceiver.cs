using Claims.Application.Channels;

namespace Claims.Application.Interfaces;

public interface IAuditMessageReceiver
{
    IAsyncEnumerable<AuditMessage> ReadAllAsync(CancellationToken cancellationToken);
}
