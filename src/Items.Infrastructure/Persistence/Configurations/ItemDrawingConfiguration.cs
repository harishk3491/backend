using Items.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Persistence.Configurations;

public class ItemDrawingConfiguration : IEntityTypeConfiguration<ItemDrawing>
{
    public void Configure(EntityTypeBuilder<ItemDrawing> builder)
    {
        builder.ToTable("ItemDrawings");

        builder.Property(x => x.DrawingName).HasMaxLength(100);
        builder.Property(x => x.DrawingNumber).HasMaxLength(100);
        builder.Property(x => x.Revision).HasMaxLength(50);
        builder.Property(x => x.Size).HasMaxLength(50);

        builder.HasOne(x => x.Item)
            .WithMany(i => i.Drawings)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
