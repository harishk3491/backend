using AutoMapper;
using Items.Application.Uoms.Dtos;
using Items.Domain.Entities;

namespace Items.Application.Uoms.Mappings;

public class UomMappingProfile : Profile
{
    public UomMappingProfile()
    {
        CreateMap<UnitOfMeasure, UomDto>();

        CreateMap<CreateUomRequest, UnitOfMeasure>(MemberList.None);

        CreateMap<UpdateUomRequest, UnitOfMeasure>(MemberList.None);
    }
}
