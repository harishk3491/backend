using Items.Domain.Enums;

namespace Items.Application.PriceLists.Dtos;

public class CreatePriceListItemRequest
{
    public int? ItemId { get; set; }
    public int? ItemClassId { get; set; }
    public decimal? MinQty { get; set; }
    public decimal? MaxQty { get; set; }
    public decimal BasePrice { get; set; }
    public DiscountType? DiscountType { get; set; }
    public decimal? DiscountValue { get; set; }
    public int? RoundOffTo { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}
