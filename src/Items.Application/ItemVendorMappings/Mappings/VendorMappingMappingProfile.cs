using AutoMapper;
using Items.Application.ItemVendorMappings.Dtos;
using Items.Domain.Entities;

namespace Items.Application.ItemVendorMappings.Mappings;

public class VendorMappingMappingProfile : Profile
{
    public VendorMappingMappingProfile()
    {
        // --- Summary (used with ProjectTo) ---
        CreateMap<ItemVendorMapping, ItemVendorMappingSummaryDto>()
            .ForMember(d => d.ItemName, o => o.MapFrom(m => m.Item.Name));

        // --- Full DTO ---
        CreateMap<ItemVendorMapping, ItemVendorMappingDto>()
            .ForMember(d => d.ItemName, o => o.MapFrom(m => m.Item.Name));

        // --- Sub-entity: PurchaseUom ---
        CreateMap<ItemVendorPurchaseUom, ItemVendorPurchaseUomDto>()
            .ForMember(d => d.PurchaseUomCode, o => o.MapFrom(u => u.PurchaseUom.Code));
        // ConversionRate override (IsPerfectConversion → 1m) handled in service after mapping
        CreateMap<CreateItemVendorPurchaseUomRequest, ItemVendorPurchaseUom>(MemberList.None);

        // --- Sub-entity: Pricing ---
        CreateMap<ItemVendorPricing, ItemVendorPricingDto>();
        // EffectivePurchasePrice computed by service after mapping
        CreateMap<CreateItemVendorPricingRequest, ItemVendorPricing>(MemberList.None);

        // --- Request → Entity: Create ---
        // PurchaseUoms and Pricings mapped separately by service (computed fields + special logic)
        CreateMap<CreateItemVendorMappingRequest, ItemVendorMapping>(MemberList.None)
            .ForMember(d => d.PurchaseUoms, o => o.Ignore())
            .ForMember(d => d.Pricings, o => o.Ignore());

        // --- Request → Entity: Update ---
        // ItemId, VendorId are non-editable post-create per SRS
        CreateMap<UpdateItemVendorMappingRequest, ItemVendorMapping>(MemberList.None)
            .ForMember(d => d.ItemId, o => o.Ignore())
            .ForMember(d => d.VendorId, o => o.Ignore())
            .ForMember(d => d.PurchaseUoms, o => o.Ignore())
            .ForMember(d => d.Pricings, o => o.Ignore());
    }
}
