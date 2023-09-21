using ContentHider.Core.Entities;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CS8618

namespace ContentHider.DAL;

public class HiderDbContext : DbContext
{
    public HiderDbContext(DbContextOptions<HiderDbContext> options)
        :
        base(options)
    {
    }

    public DbSet<OrganizationDao> Organizations { get; set; }
}