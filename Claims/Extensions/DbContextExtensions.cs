using Claims.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;


namespace Claims.Api.Extensions;

/// <summary>
/// Provides extension methods for registering database contexts.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Adds SQL Server and MongoDB database contexts to the service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="sqlConnectionString">The SQL Server connection string.</param>
    /// <param name="mongoConnectionString">The MongoDB connection string.</param>
    /// <param name="mongoDatabaseName">The MongoDB database name.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddDbContexts(this IServiceCollection services, string sqlConnectionString, string mongoConnectionString, string mongoDatabaseName)
    {
        services.AddDbContext<AuditContext>(options =>
            options.UseSqlServer(sqlConnectionString));

        services.AddDbContext<ClaimsContext>(options =>
        {
            var client = new MongoClient(mongoConnectionString);
            var database = client.GetDatabase(mongoDatabaseName);
            options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
        });

        return services;
    }
}