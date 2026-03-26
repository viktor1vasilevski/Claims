using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

namespace Claims.Integration.Tests;

public class ClaimsApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer;
    private readonly MongoDbContainer _mongoContainer;

    public ClaimsApiFactory()
    {
        _sqlContainer = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            ? new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            : new MsSqlBuilder()
        ).Build();

        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .Build();
    }

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace Service Bus with an in-process queue so integration tests
            // don't require the Service Bus emulator to be running.
            var queue = new InProcessAuditQueue();
            services.AddSingleton<IAuditMessageSender>(queue);
            services.AddSingleton<IAuditMessageReceiver>(queue);
        });
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        await _mongoContainer.StartAsync();

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("ConnectionStrings__SqlServer", _sqlContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings__MongoDb", _mongoContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("MongoDb__DatabaseName", "ClaimsTesting");

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ClaimsContext>();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
        await _mongoContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    private sealed class InProcessAuditQueue : IAuditMessageSender, IAuditMessageReceiver
    {
        private readonly Channel<AuditMessageEnvelope> _channel = Channel.CreateUnbounded<AuditMessageEnvelope>();

        public async Task SendAsync(AuditMessage message, CancellationToken cancellationToken = default)
            => await _channel.Writer.WriteAsync(new AuditMessageEnvelope(message, () => Task.CompletedTask), cancellationToken);

        public IAsyncEnumerable<AuditMessageEnvelope> ReadAllAsync(CancellationToken cancellationToken)
            => _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
