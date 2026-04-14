using Items.Domain.Common;

namespace Items.Domain.Entities;

public class ItemDrawing : BaseEntity
{
    public int ItemId { get; set; }
    public string? DrawingName { get; set; }
    public string? DrawingNumber { get; set; }
    public string? Revision { get; set; }
    public string? Size { get; set; }
    public bool ShowInProductionBooking { get; set; }

    public Item Item { get; set; } = null!;
}
