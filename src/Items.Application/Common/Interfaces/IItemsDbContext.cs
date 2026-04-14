using Microsoft.EntityFrameworkCore;

namespace Items.Application.Common.Interfaces;

public interface IItemsDbContext
{
    DbSet<Domain.Entities.ItemSettings> ItemSettings { get; }
    DbSet<Domain.Entities.UnitOfMeasure> UnitOfMeasures { get; }
    DbSet<Domain.Entities.ItemClass> ItemClasses { get; }
    DbSet<Domain.Entities.Item> Items { get; }
    DbSet<Domain.Entities.ItemAlternateUom> ItemAlternateUoms { get; }
    DbSet<Domain.Entities.ItemTechnicalSpec> ItemTechnicalSpecs { get; }
    DbSet<Domain.Entities.ItemDrawing> ItemDrawings { get; }
    DbSet<Domain.Entities.ItemWarehouseThreshold> ItemWarehouseThresholds { get; }
    DbSet<Domain.Entities.ItemVendorMapping> ItemVendorMappings { get; }
    DbSet<Domain.Entities.ItemVendorPurchaseUom> ItemVendorPurchaseUoms { get; }
    DbSet<Domain.Entities.ItemVendorPricing> ItemVendorPricings { get; }
    DbSet<Domain.Entities.PriceList> PriceLists { get; }
    DbSet<Domain.Entities.PriceListItem> PriceListItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
