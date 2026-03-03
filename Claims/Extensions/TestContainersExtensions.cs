using System.Runtime.InteropServices;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

namespace Claims.Api.Extensions;

public static class TestContainersExtensions
{
    public static async Task<(string SqlConnectionString, string MongoConnectionString)> StartContainersAsync()
    {
        var sqlContainer = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            ? new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            : new MsSqlBuilder()
        ).Build();

        var mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .Build();

        await sqlContainer.StartAsync();
        await mongoContainer.StartAsync();

        return (sqlContainer.GetConnectionString(), mongoContainer.GetConnectionString());
    }
}
