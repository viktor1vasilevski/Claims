using Claims.Application.Channels;
using Claims.Application.Interfaces;
using System.Threading.Channels;

namespace Claims.Infrastructure.Messaging;

public class InMemoryAuditQueue : IAuditMessageSender, IAuditMessageReceiver
{
    private readonly Channel<AuditMessage> _channel = Channel.CreateUnbounded<AuditMessage>();

    public async Task SendAsync(AuditMessage message, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(message, cancellationToken);
    }

    public IAsyncEnumerable<AuditMessage> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
