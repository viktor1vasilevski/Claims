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
        modelBuilder.Entity<Cover>().ToCollection("covers");

        modelBuilder.Entity<Cover>()
            .Property(x => x.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Claim>()
            .Property(x => x.Type)
            .HasConversion<string>();
    }
}