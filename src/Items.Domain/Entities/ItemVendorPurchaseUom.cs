using Items.Domain.Common;

namespace Items.Domain.Entities;

public class ItemVendorPurchaseUom : BaseEntity
{
    public int ItemVendorMappingId { get; set; }
    public int PurchaseUomId { get; set; }
    public bool IsPerfectConversion { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal TolerancePercent { get; set; }

    public ItemVendorMapping ItemVendorMapping { get; set; } = null!;
    public UnitOfMeasure PurchaseUom { get; set; } = null!;
}
