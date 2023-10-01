using ContentHider.Core.Daos;
using ContentHider.Core.Entities;
using ContentHider.Core.Enums;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618

namespace ContentHider.DAL;

public class HiderDbContext : DbContext
{
    public HiderDbContext(DbContextOptions<HiderDbContext> options) : base(options)
    {
    }

    public DbSet<OrganizationDao> Organizations { get; set; }
    public DbSet<UserDao> Users { get; set; }
    public DbSet<FormatDao> Formats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrganizationDao>(e =>
        {
            e.HasKey(i => i.Id);
            e
                .HasMany(i => i.Formats)
                .WithOne(i => i.Organization)
                .HasForeignKey(i => i.OrganizationId);
        });
        modelBuilder.Entity<FormatDao>(e => e.HasKey(i => i.Id));
        modelBuilder.Entity<UserDao>(e =>
        {
            e.HasKey(i => i.Id);

            e.HasData(new UserDao
            {
                Id = "F3994CE5-4E71-4D76-AE9B-923667D1D2C9",
                FirstName = "AdminFirstName",
                LastName = "AdminLastName",
                UserName = "admin",
                Password = "admin",
                Role = Roles.Admin
            });

            e.HasData(new UserDao
            {
                Id = "24E101BB-6F1B-45C9-ABED-3AC3ED0399DF",
                FirstName = "UserFirstName",
                LastName = "UserLastName",
                UserName = "user",
                Password = "user",
                Role = Roles.User
            });
        });
    }
}