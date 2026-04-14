using Items.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Persistence.Configurations;

public class PriceListConfiguration : IEntityTypeConfiguration<PriceList>
{
    public void Configure(EntityTypeBuilder<PriceList> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.Name).IsUnique();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(p => p.TargetIds)
            .HasMaxLength(2000);

        builder.Property(p => p.ApplicableTo)
            .HasConversion<string>();

        builder.Property(p => p.Status)
            .HasConversion<string>();

        builder.Property(p => p.TargetType)
            .HasConversion<string>();

        builder.Property(p => p.PricingScopeType)
            .HasConversion<string>();

        builder.HasMany(p => p.LineItems)
            .WithOne(li => li.PriceList)
            .HasForeignKey(li => li.PriceListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
