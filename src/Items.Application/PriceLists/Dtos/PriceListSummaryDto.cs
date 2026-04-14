using Items.Domain.Enums;

namespace Items.Application.PriceLists.Dtos;

public class PriceListSummaryDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public PriceListApplicableTo ApplicableTo { get; set; }
    public PriceListStatus Status { get; set; }
    public int Priority { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
