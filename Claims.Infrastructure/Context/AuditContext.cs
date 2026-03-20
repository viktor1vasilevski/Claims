using Claims.Domain.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Infrastructure.Context;

public class AuditContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<ClaimAudit> ClaimAudits { get; set; }
    public DbSet<CoverAudit> CoverAudits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClaimAudit>().ToCollection("claimAudits");
        modelBuilder.Entity<ClaimAudit>().Property(x => x.ClaimId).HasElementName("claimId");
        modelBuilder.Entity<ClaimAudit>().Property(x => x.Created).HasElementName("created");
        modelBuilder.Entity<ClaimAudit>().Property(x => x.HttpRequestType).HasElementName("httpRequestType").HasConversion<string>();

        modelBuilder.Entity<CoverAudit>().ToCollection("coverAudits");
        modelBuilder.Entity<CoverAudit>().Property(x => x.CoverId).HasElementName("coverId");
        modelBuilder.Entity<CoverAudit>().Property(x => x.Created).HasElementName("created");
        modelBuilder.Entity<CoverAudit>().Property(x => x.HttpRequestType).HasElementName("httpRequestType").HasConversion<string>();
    }
}
