using Azure.Messaging.ServiceBus;
using Claims.Application.Channels;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace Claims.Infrastructure.Messaging;

public class ServiceBusAuditQueue : IAuditMessageSender, IAuditMessageReceiver, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly string _queueName;
    private readonly ILogger<ServiceBusAuditQueue> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public ServiceBusAuditQueue(string connectionString, string queueName, ILogger<ServiceBusAuditQueue> logger)
        : this(new ServiceBusClient(connectionString), queueName, logger) { }

    public ServiceBusAuditQueue(ServiceBusClient client, string queueName, ILogger<ServiceBusAuditQueue> logger)
    {
        _client = client;
        _sender = client.CreateSender(queueName);
        _queueName = queueName;
        _logger = logger;
    }

    public async Task SendAsync(AuditMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var body = JsonSerializer.Serialize(message, JsonOptions);
            await _sender.SendMessageAsync(new ServiceBusMessage(body), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send audit message for {EntityType} {Id}. The audit will be skipped.",
                message.EntityType, message.Id);
        }
    }

    public async IAsyncEnumerable<AuditMessageEnvelope> ReadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<AuditMessageEnvelope>();

        var processor = _client.CreateProcessor(_queueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false
        });

        processor.ProcessMessageAsync += async args =>
        {
            AuditMessage message;
            try
            {
                message = JsonSerializer.Deserialize<AuditMessage>(args.Message.Body.ToString(), JsonOptions)!;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize audit message from Service Bus. Dead-lettering.");
                await args.DeadLetterMessageAsync(args.Message, cancellationToken: cancellationToken);
                return;
            }

            var acknowledged = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            var envelope = new AuditMessageEnvelope(message, async () =>
            {
                await args.CompleteMessageAsync(args.Message, CancellationToken.None);
                acknowledged.TrySetResult();
            });

            await channel.Writer.WriteAsync(envelope, cancellationToken);
            await acknowledged.Task;
        };

        processor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "Service Bus processor error on {EntityPath}", args.EntityPath);
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(cancellationToken);

        try
        {
            await foreach (var envelope in channel.Reader.ReadAllAsync(cancellationToken))
                yield return envelope;
        }
        finally
        {
            await processor.StopProcessingAsync();
            await processor.DisposeAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}
