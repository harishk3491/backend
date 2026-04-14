namespace Items.Application.ItemVendorMappings.Dtos;

public class ItemVendorMappingSummaryDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int VendorId { get; set; }
    public bool IsPreferredVendor { get; set; }
    public bool IsActive { get; set; }
}
