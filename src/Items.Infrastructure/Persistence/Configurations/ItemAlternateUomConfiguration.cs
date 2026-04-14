using Items.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Persistence.Configurations;

public class ItemAlternateUomConfiguration : IEntityTypeConfiguration<ItemAlternateUom>
{
    public void Configure(EntityTypeBuilder<ItemAlternateUom> builder)
    {
        builder.ToTable("ItemAlternateUoms");

        builder.Property(x => x.ConversionFactor).HasPrecision(18, 6);

        builder.HasOne(x => x.Item)
            .WithMany(i => i.AlternateUoms)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Uom)
            .WithMany()
            .HasForeignKey(x => x.UomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
