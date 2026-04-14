using Items.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Persistence.Configurations;

public class ItemSettingsConfiguration : IEntityTypeConfiguration<ItemSettings>
{
    public void Configure(EntityTypeBuilder<ItemSettings> builder)
    {
        builder.ToTable("ItemSettings");

        builder.Property(x => x.ItemCodeMaxLength)
            .IsRequired()
            .HasDefaultValue(20);

        builder.Property(x => x.AutoGenPrefix).HasMaxLength(50);
        builder.Property(x => x.AutoGenSuffix).HasMaxLength(50);
        builder.Property(x => x.AutoGenSeparator).HasMaxLength(10);

        builder.Property(x => x.CcnNumberingBasis)
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}
