using Items.Domain.Enums;

namespace Items.Application.ItemVendorMappings.Dtos;

public class CreateItemVendorPricingRequest
{
    public decimal BasePurchasePrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DiscountType? DiscountType { get; set; }
    public decimal? DiscountValue { get; set; }
    public DateOnly? PriceValidFrom { get; set; }
    public DateOnly? PriceValidTo { get; set; }
    public string PaymentTerms { get; set; } = string.Empty;
}
