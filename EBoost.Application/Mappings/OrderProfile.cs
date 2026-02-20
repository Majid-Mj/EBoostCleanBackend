using AutoMapper;
using EBoost.Application.DTOs.Order;
using EBoost.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.Mappings;

public class OrderProfile :Profile
{
    public OrderProfile()
    {
        CreateMap<Order , OrderDto >().
            ForMember(dest => dest.Status,
        opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<OrderItem, OrderItemDto>();

        CreateMap<ShippingAddress, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ShippingFullName,
                opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.ShippingPhone,
                opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.ShippingStreet,
                opt => opt.MapFrom(src => src.Street))
            .ForMember(dest => dest.ShippingCity,
                opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.ShippingState,
                opt => opt.MapFrom(src => src.State))
            .ForMember(dest => dest.ShippingPostalCode,
                opt => opt.MapFrom(src => src.PostalCode))
            .ForMember(dest => dest.ShippingCountry,
                opt => opt.MapFrom(src => src.Country));
            //.ForAllOtherMembers(opt => opt.Ignore());
    }
}
