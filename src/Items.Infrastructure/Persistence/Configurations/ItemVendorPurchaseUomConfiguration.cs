using Items.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Persistence.Configurations;

public class ItemVendorPurchaseUomConfiguration : IEntityTypeConfiguration<ItemVendorPurchaseUom>
{
    public void Configure(EntityTypeBuilder<ItemVendorPurchaseUom> builder)
    {
        builder.ToTable("ItemVendorPurchaseUoms");

        builder.Property(x => x.ConversionRate).HasPrecision(18, 6);
        builder.Property(x => x.TolerancePercent).HasPrecision(5, 2);

        builder.HasOne(x => x.ItemVendorMapping)
            .WithMany(m => m.PurchaseUoms)
            .HasForeignKey(x => x.ItemVendorMappingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PurchaseUom)
            .WithMany()
            .HasForeignKey(x => x.PurchaseUomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
