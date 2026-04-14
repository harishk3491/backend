using Items.Domain.Enums;

namespace Items.Application.ItemClasses.Dtos;

public class ItemClassSummaryDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ItemNature ItemNature { get; set; }
    public ItemType? ItemType { get; set; }
    public bool IsActive { get; set; }
}
