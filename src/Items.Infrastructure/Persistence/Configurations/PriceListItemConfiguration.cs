using Items.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Persistence.Configurations;

public class PriceListItemConfiguration : IEntityTypeConfiguration<PriceListItem>
{
    public void Configure(EntityTypeBuilder<PriceListItem> builder)
    {
        builder.HasKey(li => li.Id);

        builder.Property(li => li.BasePrice)
            .HasPrecision(18, 4);

        builder.Property(li => li.DiscountValue)
            .HasPrecision(18, 4);

        builder.Property(li => li.FinalUnitPrice)
            .HasPrecision(18, 4);

        builder.Property(li => li.MinQty)
            .HasPrecision(18, 4);

        builder.Property(li => li.MaxQty)
            .HasPrecision(18, 4);

        builder.Property(li => li.DiscountType)
            .HasConversion<string>();

        builder.HasOne(li => li.Item)
            .WithMany()
            .HasForeignKey(li => li.ItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(li => li.ItemClass)
            .WithMany()
            .HasForeignKey(li => li.ItemClassId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
