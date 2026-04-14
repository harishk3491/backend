namespace Items.Application.Items.Dtos;

public class CreateItemDrawingRequest
{
    public string? DrawingName { get; set; }
    public string? DrawingNumber { get; set; }
    public string? Revision { get; set; }
    public string? Size { get; set; }
    public bool ShowInProductionBooking { get; set; }
}
