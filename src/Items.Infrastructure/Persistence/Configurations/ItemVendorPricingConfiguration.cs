using Items.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Persistence.Configurations;

public class ItemVendorPricingConfiguration : IEntityTypeConfiguration<ItemVendorPricing>
{
    public void Configure(EntityTypeBuilder<ItemVendorPricing> builder)
    {
        builder.ToTable("ItemVendorPricings");

        builder.Property(x => x.BasePurchasePrice).HasPrecision(18, 4);
        builder.Property(x => x.DiscountValue).HasPrecision(18, 4);
        builder.Property(x => x.EffectivePurchasePrice).HasPrecision(18, 4);
        builder.Property(x => x.Currency).HasMaxLength(10).IsRequired();
        builder.Property(x => x.PaymentTerms).HasMaxLength(100).IsRequired();

        builder.HasOne(x => x.ItemVendorMapping)
            .WithMany(m => m.Pricings)
            .HasForeignKey(x => x.ItemVendorMappingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
