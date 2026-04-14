using Items.Domain.Common;

namespace Items.Domain.Entities;

public class ItemAlternateUom : BaseEntity
{
    public int ItemId { get; set; }
    public int UomId { get; set; }
    public decimal ConversionFactor { get; set; }
    public bool IsPerfect { get; set; }

    public Item Item { get; set; } = null!;
    public UnitOfMeasure Uom { get; set; } = null!;
}
