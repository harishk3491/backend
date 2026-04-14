using Items.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Persistence.Configurations;

public class ItemWarehouseThresholdConfiguration : IEntityTypeConfiguration<ItemWarehouseThreshold>
{
    public void Configure(EntityTypeBuilder<ItemWarehouseThreshold> builder)
    {
        builder.ToTable("ItemWarehouseThresholds");

        builder.Property(x => x.WarehouseName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.MinThreshold).HasPrecision(18, 4);
        builder.Property(x => x.MaxThreshold).HasPrecision(18, 4);
        builder.Property(x => x.ReorderQty).HasPrecision(18, 4);
        builder.Property(x => x.OpeningQty).HasPrecision(18, 4);
        builder.Property(x => x.OpeningRate).HasPrecision(18, 4);

        builder.HasOne(x => x.Item)
            .WithMany(i => i.WarehouseThresholds)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
