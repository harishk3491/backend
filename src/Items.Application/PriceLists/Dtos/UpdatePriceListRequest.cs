using Items.Domain.Enums;

namespace Items.Application.PriceLists.Dtos;

public class UpdatePriceListRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PriceListStatus Status { get; set; }
    public int Priority { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PriceListTargetType TargetType { get; set; }
    public string? TargetIds { get; set; }
    public PricingScopeType PricingScopeType { get; set; }
    public List<CreatePriceListItemRequest> LineItems { get; set; } = [];
}
