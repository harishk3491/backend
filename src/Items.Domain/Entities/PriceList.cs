using Items.Domain.Common;
using Items.Domain.Enums;

namespace Items.Domain.Entities;

public class PriceList : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    // Section 1: Header
    public PriceListApplicableTo ApplicableTo { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PriceListStatus Status { get; set; } = PriceListStatus.Draft;
    public int Priority { get; set; }
    public string Currency { get; set; } = string.Empty;

    // Section 2: Applicability
    public PriceListTargetType TargetType { get; set; }
    public string? TargetIds { get; set; }   // JSON array of customer/vendor/region IDs

    // Section 3: Pricing Scope
    public PricingScopeType PricingScopeType { get; set; }

    // Navigation
    public ICollection<PriceListItem> LineItems { get; set; } = [];
}
