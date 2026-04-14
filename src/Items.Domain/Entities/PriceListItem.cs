using Items.Domain.Common;
using Items.Domain.Enums;

namespace Items.Domain.Entities;

public class PriceListItem : BaseEntity
{
    public int PriceListId { get; set; }
    public int? ItemId { get; set; }
    public int? ItemClassId { get; set; }

    // Slab
    public decimal? MinQty { get; set; }
    public decimal? MaxQty { get; set; }

    // Pricing
    public decimal BasePrice { get; set; }
    public DiscountType? DiscountType { get; set; }
    public decimal? DiscountValue { get; set; }
    public decimal FinalUnitPrice { get; set; }   // computed: BasePrice - Discount
    public int? RoundOffTo { get; set; }          // decimal places

    // Validity
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    // Navigation
    public PriceList PriceList { get; set; } = null!;
    public Item? Item { get; set; }
    public ItemClass? ItemClass { get; set; }
}
