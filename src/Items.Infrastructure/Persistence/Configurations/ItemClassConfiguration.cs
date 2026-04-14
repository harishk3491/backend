using Items.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Persistence.Configurations;

public class ItemClassConfiguration : IEntityTypeConfiguration<ItemClass>
{
    public void Configure(EntityTypeBuilder<ItemClass> builder)
    {
        builder.ToTable("ItemClasses");

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.AllowExcessGrnPercent)
            .HasPrecision(5, 2);

        builder.Property(x => x.FastMovingThreshold)
            .HasPrecision(18, 4);

        builder.Property(x => x.SlowMovingThreshold)
            .HasPrecision(18, 4);

        builder.Property(x => x.MinimumProductionQty)
            .HasPrecision(18, 4);

        builder.Property(x => x.ProductionBatchMultiple)
            .HasPrecision(18, 4);

        builder.Property(x => x.SalesAccountGroup)
            .HasMaxLength(100);

        builder.Property(x => x.SalesAccount)
            .HasMaxLength(100);

        builder.Property(x => x.PurchaseAccountGroup)
            .HasMaxLength(100);

        builder.Property(x => x.PurchaseAccount)
            .HasMaxLength(100);

        builder.HasIndex(x => x.Code).IsUnique();
    }
}
