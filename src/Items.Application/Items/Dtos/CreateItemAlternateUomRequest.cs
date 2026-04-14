namespace Items.Application.Items.Dtos;

public class CreateItemAlternateUomRequest
{
    public int UomId { get; set; }
    public decimal ConversionFactor { get; set; }
    public bool IsPerfect { get; set; }
}
