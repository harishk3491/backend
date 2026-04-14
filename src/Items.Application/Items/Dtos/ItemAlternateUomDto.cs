namespace Items.Application.Items.Dtos;

public class ItemAlternateUomDto
{
    public int Id { get; set; }
    public int UomId { get; set; }
    public string UomCode { get; set; } = string.Empty;
    public decimal ConversionFactor { get; set; }
    public bool IsPerfect { get; set; }
}
