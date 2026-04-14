using AutoMapper;
using Items.Application.ItemClasses.Dtos;
using Items.Domain.Entities;

namespace Items.Application.ItemClasses.Mappings;

public class ItemClassMappingProfile : Profile
{
    public ItemClassMappingProfile()
    {
        CreateMap<ItemClass, ItemClassSummaryDto>();

        CreateMap<ItemClass, ItemClassDto>();

        CreateMap<CreateItemClassRequest, ItemClass>(MemberList.None);

        // ItemNature is non-editable post-create per SRS — ignore even if present in request
        CreateMap<UpdateItemClassRequest, ItemClass>(MemberList.None)
            .ForMember(d => d.ItemNature, o => o.Ignore());
    }
}
