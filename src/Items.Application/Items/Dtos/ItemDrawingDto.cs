namespace Items.Application.Items.Dtos;

public class ItemDrawingDto
{
    public int Id { get; set; }
    public string? DrawingName { get; set; }
    public string? DrawingNumber { get; set; }
    public string? Revision { get; set; }
    public string? Size { get; set; }
    public bool ShowInProductionBooking { get; set; }
}
