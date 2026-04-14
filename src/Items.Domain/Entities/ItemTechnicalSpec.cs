using Items.Domain.Common;

namespace Items.Domain.Entities;

public class ItemTechnicalSpec : BaseEntity
{
    public int ItemId { get; set; }
    public string CapabilityType { get; set; } = string.Empty;
    public string CapabilityValue { get; set; } = string.Empty;

    public Item Item { get; set; } = null!;
}
