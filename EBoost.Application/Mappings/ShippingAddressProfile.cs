using AutoMapper;
using EBoost.Application.DTOs.Address;
using EBoost.Domain.Entities;

namespace EBoost.Application.Mappings;

public  class ShippingAddressProfile : Profile
{
    public ShippingAddressProfile()
    {

        CreateMap<ShippingAddress, AddressDto>();

        // Create DTO → Entity
        CreateMap<CreateAddressDto, ShippingAddress>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<UpdateAddressDto, ShippingAddress>()
           .ForMember(dest => dest.Id, opt => opt.Ignore())
           .ForMember(dest => dest.UserId, opt => opt.Ignore())
           .ForMember(dest => dest.IsDefault, opt => opt.Ignore())
           .ForMember(dest => dest.User, opt => opt.Ignore());
    }
         
}
