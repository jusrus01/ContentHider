using ContentHider.Core.Entities;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618

namespace ContentHider.DAL;

public class HiderDbContext : DbContext
{
    public DbSet<OrganizationDao> Organizations { get; set; }

    public HiderDbContext(DbContextOptions<HiderDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrganizationDao>(e =>
        {
            e.HasKey(i => i.Id);
        });
    }
}