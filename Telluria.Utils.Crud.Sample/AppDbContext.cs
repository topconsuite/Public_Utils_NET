using Microsoft.EntityFrameworkCore;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Services;

namespace Telluria.Utils.Crud.Sample
{
  public class AppDbContext : DbContext
  {
    private readonly Guid _tenantId;

    public AppDbContext(ITenantService tenantService, DbContextOptions<AppDbContext> options) : base(options)
    {
      _tenantId = tenantService.TenantId;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      => optionsBuilder.UseSqlite("DataSource=app.db;Cache=Shared");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfiguration(new ProductMap());

      modelBuilder.Entity<Product>().HasQueryFilter(a => a.TenantId == _tenantId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
      foreach (var entry in ChangeTracker.Entries())
      {
        switch (entry.State)
        {
          case EntityState.Added:
          case EntityState.Modified:
            entry.Property("TenantId").CurrentValue = _tenantId;
            break;
        }
      }

      return await base.SaveChangesAsync(cancellationToken);
    }
  }
}
