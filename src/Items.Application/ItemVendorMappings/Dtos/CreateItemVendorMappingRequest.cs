using Items.Domain.Enums;

namespace Items.Application.ItemVendorMappings.Dtos;

public class CreateItemVendorMappingRequest
{
    public int ItemId { get; set; }
    public int VendorId { get; set; }
    public string? VendorItemCode { get; set; }
    public string? VendorSku { get; set; }
    public bool IsPreferredVendor { get; set; }

    public decimal Moq { get; set; }
    public decimal? MaxOrderQty { get; set; }
    public decimal? Eoq { get; set; }
    public int StandardLeadTimeDays { get; set; }
    public int? WarrantyDuration { get; set; }
    public WarrantyDurationUnit? WarrantyDurationUnit { get; set; }
    public WarrantyStartBasis? WarrantyStartBasis { get; set; }

    public List<CreateItemVendorPurchaseUomRequest> PurchaseUoms { get; set; } = [];
    public List<CreateItemVendorPricingRequest> Pricings { get; set; } = [];
}
