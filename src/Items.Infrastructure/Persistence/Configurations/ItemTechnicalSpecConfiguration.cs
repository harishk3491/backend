using Items.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Persistence.Configurations;

public class ItemTechnicalSpecConfiguration : IEntityTypeConfiguration<ItemTechnicalSpec>
{
    public void Configure(EntityTypeBuilder<ItemTechnicalSpec> builder)
    {
        builder.ToTable("ItemTechnicalSpecs");

        builder.Property(x => x.CapabilityType).IsRequired().HasMaxLength(200);
        builder.Property(x => x.CapabilityValue).IsRequired().HasMaxLength(500);

        builder.HasOne(x => x.Item)
            .WithMany(i => i.TechnicalSpecs)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
