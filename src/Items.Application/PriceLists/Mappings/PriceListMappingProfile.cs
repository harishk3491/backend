using AutoMapper;
using Items.Application.PriceLists.Dtos;
using Items.Domain.Entities;

namespace Items.Application.PriceLists.Mappings;

public class PriceListMappingProfile : Profile
{
    public PriceListMappingProfile()
    {
        // --- Summary (used with ProjectTo) ---
        CreateMap<PriceList, PriceListSummaryDto>();

        // --- Full DTO ---
        // LineItems sorted by MinQty per SRS display requirement
        CreateMap<PriceList, PriceListDto>()
            .ForMember(d => d.LineItems, o => o.MapFrom(p => p.LineItems.OrderBy(li => li.MinQty)));

        // --- Sub-entity: PriceListItem ---
        CreateMap<PriceListItem, PriceListItemDto>()
            .ForMember(d => d.ItemName, o => o.MapFrom(li => li.Item != null ? li.Item.Name : null))
            .ForMember(d => d.ItemClassName, o => o.MapFrom(li => li.ItemClass != null ? li.ItemClass.Name : null));
        // FinalUnitPrice computed by service after mapping
        CreateMap<CreatePriceListItemRequest, PriceListItem>(MemberList.None);

        // --- Request → Entity: Create ---
        // LineItems mapped separately by service (FinalUnitPrice computed per item)
        CreateMap<CreatePriceListRequest, PriceList>(MemberList.None)
            .ForMember(d => d.LineItems, o => o.Ignore());

        // --- Request → Entity: Update ---
        // ApplicableTo is not updated post-create; LineItems handled separately
        CreateMap<UpdatePriceListRequest, PriceList>(MemberList.None)
            .ForMember(d => d.ApplicableTo, o => o.Ignore())
            .ForMember(d => d.LineItems, o => o.Ignore());
    }
}
