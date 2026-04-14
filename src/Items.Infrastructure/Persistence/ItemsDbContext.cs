using Items.Application.Common.Interfaces;
using Items.Domain.Common;
using Items.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Items.Infrastructure.Persistence;

public class ItemsDbContext : DbContext, IItemsDbContext
{
    public DbSet<ItemSettings> ItemSettings => Set<ItemSettings>();
    public DbSet<UnitOfMeasure> UnitOfMeasures => Set<UnitOfMeasure>();
    public DbSet<ItemClass> ItemClasses => Set<ItemClass>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemAlternateUom> ItemAlternateUoms => Set<ItemAlternateUom>();
    public DbSet<ItemTechnicalSpec> ItemTechnicalSpecs => Set<ItemTechnicalSpec>();
    public DbSet<ItemDrawing> ItemDrawings => Set<ItemDrawing>();
    public DbSet<ItemWarehouseThreshold> ItemWarehouseThresholds => Set<ItemWarehouseThreshold>();
    public DbSet<ItemVendorMapping> ItemVendorMappings => Set<ItemVendorMapping>();
    public DbSet<ItemVendorPurchaseUom> ItemVendorPurchaseUoms => Set<ItemVendorPurchaseUom>();
    public DbSet<ItemVendorPricing> ItemVendorPricings => Set<ItemVendorPricing>();
    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<PriceListItem> PriceListItems => Set<PriceListItem>();

    public ItemsDbContext(DbContextOptions<ItemsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ItemsDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditFields()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}
