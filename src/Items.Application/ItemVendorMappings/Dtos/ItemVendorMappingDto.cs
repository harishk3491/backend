using Items.Domain.Enums;

namespace Items.Application.ItemVendorMappings.Dtos;

public class ItemVendorMappingDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;

    // Core Identification
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int VendorId { get; set; }
    public string? VendorItemCode { get; set; }
    public string? VendorSku { get; set; }
    public bool IsPreferredVendor { get; set; }

    // Logistics
    public decimal Moq { get; set; }
    public decimal? MaxOrderQty { get; set; }
    public decimal? Eoq { get; set; }
    public int StandardLeadTimeDays { get; set; }
    public int? WarrantyDuration { get; set; }
    public WarrantyDurationUnit? WarrantyDurationUnit { get; set; }
    public WarrantyStartBasis? WarrantyStartBasis { get; set; }

    public bool IsActive { get; set; }
    public List<ItemVendorPurchaseUomDto> PurchaseUoms { get; set; } = [];
    public List<ItemVendorPricingDto> Pricings { get; set; } = [];
}
