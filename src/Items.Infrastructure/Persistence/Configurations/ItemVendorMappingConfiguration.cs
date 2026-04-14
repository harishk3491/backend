using Items.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Persistence.Configurations;

public class ItemVendorMappingConfiguration : IEntityTypeConfiguration<ItemVendorMapping>
{
    public void Configure(EntityTypeBuilder<ItemVendorMapping> builder)
    {
        builder.ToTable("ItemVendorMappings");

        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Code).HasMaxLength(20).IsRequired();

        builder.HasIndex(x => new { x.ItemId, x.VendorId }).IsUnique();

        builder.Property(x => x.VendorItemCode).HasMaxLength(100);
        builder.Property(x => x.VendorSku).HasMaxLength(100);

        builder.Property(x => x.Moq).HasPrecision(18, 4);
        builder.Property(x => x.MaxOrderQty).HasPrecision(18, 4);
        builder.Property(x => x.Eoq).HasPrecision(18, 4);

        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
