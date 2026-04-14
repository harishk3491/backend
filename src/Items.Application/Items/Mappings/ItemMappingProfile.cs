using AutoMapper;
using Items.Application.Items.Dtos;
using Items.Domain.Entities;

namespace Items.Application.Items.Mappings;

public class ItemMappingProfile : Profile
{
    public ItemMappingProfile()
    {
        // --- Summary (used with ProjectTo in GetAllAsync) ---
        CreateMap<Item, ItemSummaryDto>()
            .ForMember(d => d.ItemClassName, o => o.MapFrom(i => i.ItemClass != null ? i.ItemClass.Name : null))
            .ForMember(d => d.BaseUomCode, o => o.MapFrom(i => i.BaseUom.Code));

        // --- Full DTO (used with _mapper.Map after Include loads) ---
        CreateMap<Item, ItemDto>()
            .ForMember(d => d.ItemClassName, o => o.MapFrom(i => i.ItemClass != null ? i.ItemClass.Name : null))
            .ForMember(d => d.BaseUomCode, o => o.MapFrom(i => i.BaseUom.Code))
            .ForMember(d => d.AlternateUoms, o => o.MapFrom(i => i.AlternateUoms.Where(a => a.IsActive)))
            .ForMember(d => d.TechnicalSpecs, o => o.MapFrom(i => i.TechnicalSpecs.Where(s => s.IsActive)))
            .ForMember(d => d.Drawings, o => o.MapFrom(i => i.Drawings.Where(d2 => d2.IsActive)))
            .ForMember(d => d.WarehouseThresholds, o => o.MapFrom(i => i.WarehouseThresholds.Where(w => w.IsActive)));

        // --- Sub-entity: AlternateUom ---
        CreateMap<ItemAlternateUom, ItemAlternateUomDto>()
            .ForMember(d => d.UomCode, o => o.MapFrom(a => a.Uom.Code));
        CreateMap<CreateItemAlternateUomRequest, ItemAlternateUom>(MemberList.None);

        // --- Sub-entity: TechnicalSpec ---
        CreateMap<ItemTechnicalSpec, ItemTechnicalSpecDto>();
        CreateMap<CreateItemTechnicalSpecRequest, ItemTechnicalSpec>(MemberList.None);

        // --- Sub-entity: Drawing ---
        CreateMap<ItemDrawing, ItemDrawingDto>();
        CreateMap<CreateItemDrawingRequest, ItemDrawing>(MemberList.None);

        // --- Sub-entity: WarehouseThreshold ---
        CreateMap<ItemWarehouseThreshold, ItemWarehouseThresholdDto>();
        CreateMap<CreateItemWarehouseThresholdRequest, ItemWarehouseThreshold>(MemberList.None);

        // --- Request → Entity: Create ---
        // Sub-collections are ignored here; service maps them separately via _mapper.Map<List<T>>
        CreateMap<CreateItemRequest, Item>(MemberList.None)
            .ForMember(d => d.AlternateUoms, o => o.Ignore())
            .ForMember(d => d.TechnicalSpecs, o => o.Ignore())
            .ForMember(d => d.Drawings, o => o.Ignore())
            .ForMember(d => d.WarehouseThresholds, o => o.Ignore());

        // --- Request → Entity: Update ---
        // ItemNature, dimension flags are non-editable post-create per SRS
        CreateMap<UpdateItemRequest, Item>(MemberList.None)
            .ForMember(d => d.ItemNature, o => o.Ignore())
            .ForMember(d => d.DimensionBasedItem, o => o.Ignore())
            .ForMember(d => d.NeedDimensionWiseStockKeeping, o => o.Ignore())
            .ForMember(d => d.NeedDimensionWiseConsumptionInBom, o => o.Ignore())
            .ForMember(d => d.LongItem, o => o.Ignore())
            .ForMember(d => d.AlternateUoms, o => o.Ignore())
            .ForMember(d => d.TechnicalSpecs, o => o.Ignore())
            .ForMember(d => d.Drawings, o => o.Ignore())
            .ForMember(d => d.WarehouseThresholds, o => o.Ignore());
    }
}
