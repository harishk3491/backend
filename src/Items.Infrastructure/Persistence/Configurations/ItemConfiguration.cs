using Items.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Persistence.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");

        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Sku).IsRequired().HasMaxLength(100);
        builder.Property(x => x.VariantName).HasMaxLength(100);
        builder.Property(x => x.VariantCode).HasMaxLength(50);

        builder.Property(x => x.ReferenceCode).HasMaxLength(100);
        builder.Property(x => x.Manufacturer).HasMaxLength(200);

        builder.Property(x => x.MinMassManufacturingQty).HasPrecision(18, 4);
        builder.Property(x => x.AllowExcessGrnPercent).HasPrecision(5, 2);
        builder.Property(x => x.FastMovingThreshold).HasPrecision(18, 4);
        builder.Property(x => x.SlowMovingThreshold).HasPrecision(18, 4);
        builder.Property(x => x.MinimumProductionQty).HasPrecision(18, 4);
        builder.Property(x => x.ProductionBatchMultiple).HasPrecision(18, 4);

        builder.Property(x => x.MaterialOfConstruction).IsRequired().HasMaxLength(200);
        builder.Property(x => x.StandardWeight).HasPrecision(18, 4);

        builder.Property(x => x.HsnSacCode).IsRequired().HasMaxLength(20);
        builder.Property(x => x.CountryOfOrigin).IsRequired().HasMaxLength(100);
        builder.Property(x => x.TaxRate).HasPrecision(5, 2);

        builder.Property(x => x.PurchaseCost).HasPrecision(18, 4);
        builder.Property(x => x.SalesPrice).HasPrecision(18, 4);
        builder.Property(x => x.SubContractingCost).HasPrecision(18, 4);
        builder.Property(x => x.MaximumCapacity).HasPrecision(18, 4);

        builder.Property(x => x.SalesAccountGroup).HasMaxLength(100);
        builder.Property(x => x.SalesAccount).HasMaxLength(100);
        builder.Property(x => x.PurchaseAccountGroup).HasMaxLength(100);
        builder.Property(x => x.PurchaseAccount).HasMaxLength(100);

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.Sku).IsUnique();

        builder.HasOne(x => x.ItemClass)
            .WithMany()
            .HasForeignKey(x => x.ItemClassId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.ParentItem)
            .WithMany()
            .HasForeignKey(x => x.ParentItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BaseUom)
            .WithMany()
            .HasForeignKey(x => x.BaseUomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
