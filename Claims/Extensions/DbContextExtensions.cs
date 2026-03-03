using Claims.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Claims.Api.Extensions;

public static class DbContextExtensions
{
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
