using Items.Domain.Common;
using Items.Domain.Enums;

namespace Items.Domain.Entities;

public class ItemVendorPricing : BaseEntity
{
    public int ItemVendorMappingId { get; set; }
    public decimal BasePurchasePrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DiscountType? DiscountType { get; set; }
    public decimal? DiscountValue { get; set; }
    public decimal EffectivePurchasePrice { get; set; }
    public DateOnly? PriceValidFrom { get; set; }
    public DateOnly? PriceValidTo { get; set; }
    public string PaymentTerms { get; set; } = string.Empty;

    public ItemVendorMapping ItemVendorMapping { get; set; } = null!;
}
