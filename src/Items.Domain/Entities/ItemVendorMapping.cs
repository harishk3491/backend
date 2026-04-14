using Items.Domain.Common;
using Items.Domain.Enums;

namespace Items.Domain.Entities;

public class ItemVendorMapping : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    // Section 1: Core Identification
    public int ItemId { get; set; }
    public int VendorId { get; set; }                   // External — no nav property
    public string? VendorItemCode { get; set; }
    public string? VendorSku { get; set; }
    public bool IsPreferredVendor { get; set; }

    // Section 3: Purchase & Logistics Details
    public decimal Moq { get; set; }
    public decimal? MaxOrderQty { get; set; }
    public decimal? Eoq { get; set; }
    public int StandardLeadTimeDays { get; set; }
    public int? WarrantyDuration { get; set; }
    public WarrantyDurationUnit? WarrantyDurationUnit { get; set; }
    public WarrantyStartBasis? WarrantyStartBasis { get; set; }

    // Navigation
    public Item Item { get; set; } = null!;
    public ICollection<ItemVendorPurchaseUom> PurchaseUoms { get; set; } = [];
    public ICollection<ItemVendorPricing> Pricings { get; set; } = [];
}
