using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc.Testing;
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

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        await _mongoContainer.StartAsync();

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Staging");
        Environment.SetEnvironmentVariable("ConnectionStrings__SqlServer", _sqlContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings__MongoDb", _mongoContainer.GetConnectionString());
    }

    public new async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
        await _mongoContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
