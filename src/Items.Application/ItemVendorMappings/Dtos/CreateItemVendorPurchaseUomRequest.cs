namespace Items.Application.ItemVendorMappings.Dtos;

public class CreateItemVendorPurchaseUomRequest
{
    public int PurchaseUomId { get; set; }
    public bool IsPerfectConversion { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal TolerancePercent { get; set; }
}
