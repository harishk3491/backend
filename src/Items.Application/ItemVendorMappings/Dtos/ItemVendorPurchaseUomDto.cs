namespace Items.Application.ItemVendorMappings.Dtos;

public class ItemVendorPurchaseUomDto
{
    public int Id { get; set; }
    public int PurchaseUomId { get; set; }
    public string PurchaseUomCode { get; set; } = string.Empty;
    public bool IsPerfectConversion { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal TolerancePercent { get; set; }
}
