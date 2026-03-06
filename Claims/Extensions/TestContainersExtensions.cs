using System.Runtime.InteropServices;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

namespace Claims.Api.Extensions;

/// <summary>
/// Provides extension methods for starting TestContainers.
/// </summary>
public static class TestContainersExtensions
{
    /// <summary>
    /// Starts SQL Server and MongoDB containers and returns their connection strings.
    /// </summary>
    /// <returns>A tuple containing the SQL Server and MongoDB connection strings.</returns>
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