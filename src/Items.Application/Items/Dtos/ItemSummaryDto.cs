using Items.Domain.Enums;

namespace Items.Application.Items.Dtos;

public class ItemSummaryDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? ItemClassName { get; set; }
    public ItemNature? ItemNature { get; set; }
    public ItemType? ItemType { get; set; }
    public string BaseUomCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
