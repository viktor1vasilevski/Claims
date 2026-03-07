using Claims.Domain.Exceptions;
using Claims.Domain.Models;
using Claims.Infrastructure.Context;
using Claims.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Tests;

public class ClaimsRepositoryTests
{
    private ClaimsContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ClaimsContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TestClaimsContext(options);
    }

    private class TestClaimsContext(DbContextOptions options) : ClaimsContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Claim>().Property(x => x.Type).HasConversion<string>();
            modelBuilder.Entity<Cover>().Property(x => x.Type).HasConversion<string>();
        }
    }

    [Fact]
    public async Task DeleteClaimAsync_WhenClaimDoesNotExist_ShouldThrowClaimNotFoundException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var sut = new ClaimsRepository(context);

        // Act
        var act = async () => await sut.DeleteClaimAsync("non-existent-id");

        // Assert
        await act.Should().ThrowAsync<ClaimNotFoundException>();
    }
}