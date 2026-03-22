using System.Threading.Channels;

namespace Claims.Application.Channels;

public record AuditMessage(string Id, HttpRequestType HttpRequestType, AuditEntityType EntityType);

public class AuditChannel
{
    private readonly Channel<AuditMessage> _channel = Channel.CreateUnbounded<AuditMessage>();
    public ChannelWriter<AuditMessage> Writer => _channel.Writer;
    public ChannelReader<AuditMessage> Reader => _channel.Reader;
}
