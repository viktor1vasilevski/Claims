using Claims.Domain.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Infrastructure.Context;

public class ClaimsContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Claim> Claims { get; init; }
    public DbSet<Cover> Covers { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Claim>().ToCollection("claims");
        modelBuilder.Entity<Claim>().Property(x => x.CoverId).HasElementName("coverId");
        modelBuilder.Entity<Claim>().Property(x => x.Created).HasElementName("created");
        modelBuilder.Entity<Claim>().Property(x => x.Name).HasElementName("name");
        modelBuilder.Entity<Claim>().Property(x => x.DamageCost).HasElementName("damageCost");
        modelBuilder.Entity<Claim>().Property(x => x.Type).HasElementName("claimType").HasConversion<string>();

        modelBuilder.Entity<Cover>().ToCollection("covers");
        modelBuilder.Entity<Cover>().Property(x => x.StartDate).HasElementName("startDate");
        modelBuilder.Entity<Cover>().Property(x => x.EndDate).HasElementName("endDate");
        modelBuilder.Entity<Cover>().Property(x => x.Premium).HasElementName("premium");
        modelBuilder.Entity<Cover>().Property(x => x.Type).HasElementName("coverType").HasConversion<string>();
    }
}