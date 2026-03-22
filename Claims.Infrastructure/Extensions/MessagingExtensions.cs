using Claims.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Claims.Infrastructure.Extensions;

public static class MessagingExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["Messaging:ServiceBus:ConnectionString"]!;
        var queueName = configuration["Messaging:ServiceBus:QueueName"]!;

        services.AddSingleton(sp => new ServiceBusAuditQueue(
            connectionString,
            queueName,
            sp.GetRequiredService<ILogger<ServiceBusAuditQueue>>()));

        services.AddSingleton<IAuditMessageSender>(sp => sp.GetRequiredService<ServiceBusAuditQueue>());
        services.AddSingleton<IAuditMessageReceiver>(sp => sp.GetRequiredService<ServiceBusAuditQueue>());

        return services;
    }
}
