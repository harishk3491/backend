using AutoMapper;
using Items.Application.ItemSettings.Dtos;

namespace Items.Application.ItemSettings.Mappings;

public class ItemSettingsMappingProfile : Profile
{
    public ItemSettingsMappingProfile()
    {
        CreateMap<Domain.Entities.ItemSettings, ItemSettingsDto>();

        CreateMap<UpdateItemSettingsRequest, Domain.Entities.ItemSettings>(MemberList.None);
    }
}
