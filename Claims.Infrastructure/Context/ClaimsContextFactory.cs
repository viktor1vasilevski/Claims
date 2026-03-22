using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Claims.Infrastructure.Context;

public class ClaimsContextFactory : IDesignTimeDbContextFactory<ClaimsContext>
{
    public ClaimsContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("ConnectionStrings:SqlServer environment variable is not set.");

        var options = new DbContextOptionsBuilder<ClaimsContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new ClaimsContext(options);
    }
}
