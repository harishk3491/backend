namespace Items.Application.Items.Dtos;

public class ItemWarehouseThresholdDto
{
    public int Id { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public decimal MinThreshold { get; set; }
    public decimal MaxThreshold { get; set; }
    public decimal ReorderQty { get; set; }
    public decimal? OpeningQty { get; set; }
    public decimal? OpeningRate { get; set; }
    public DateTime? OpeningDate { get; set; }
}
