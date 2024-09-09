using AuthenticationBase.EntityFramework.Configurations;
using AuthenticationBase.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationBase.EntityFramework;

public sealed class ApplicationDbContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
    }
}