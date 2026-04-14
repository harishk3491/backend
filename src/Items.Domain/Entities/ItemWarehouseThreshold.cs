using Items.Domain.Common;

namespace Items.Domain.Entities;

public class ItemWarehouseThreshold : BaseEntity
{
    public int ItemId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public decimal MinThreshold { get; set; }
    public decimal MaxThreshold { get; set; }
    public decimal ReorderQty { get; set; }
    public decimal? OpeningQty { get; set; }
    public decimal? OpeningRate { get; set; }
    public DateTime? OpeningDate { get; set; }

    public Item Item { get; set; } = null!;
}
