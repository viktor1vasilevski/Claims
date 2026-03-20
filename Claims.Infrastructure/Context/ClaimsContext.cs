using Claims.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Claims.Infrastructure.Context;

public class ClaimsContext(DbContextOptions<ClaimsContext> options) : DbContext(options)
{
    public DbSet<Claim> Claims { get; init; }
    public DbSet<Cover> Covers { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
