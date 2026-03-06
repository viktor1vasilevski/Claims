using Claims.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Context;

public class AuditContext : DbContext
{
    public AuditContext(DbContextOptions<AuditContext> options) : base(options)
    {
    }

    public DbSet<ClaimAudit> ClaimAudits { get; set; }
    public DbSet<CoverAudit> CoverAudits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClaimAudit>()
            .Property(x => x.HttpRequestType)
            .HasConversion<string>();

        modelBuilder.Entity<CoverAudit>()
            .Property(x => x.HttpRequestType)
            .HasConversion<string>();
    }
}
