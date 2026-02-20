using AutoMapper;
using EBoost.Application.DTOs.Cart;
using EBoost.Domain.Entities;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<CartItem, EBoost.Application.DTOs.Cart.CartItemDto>()
            .ForMember(d => d.ProductName,
                o => o.MapFrom(s => s.Product.Name))
            .ForMember(d => d.Price,
                o => o.MapFrom(s => s.Product.Price))
            .ForMember(d => d.TotalPrice,
                o => o.MapFrom(s => s.Product.Price * s.Quantity))
            .ForMember(d => d.ImageUrl,
                o => o.MapFrom(s =>
                    s.Product.Images
                        .Where(i => i.IsPrimary)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()));    

        CreateMap<Cart, CartDto>()
            .ForMember(d => d.CartId,
                o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Items,
                o => o.MapFrom(s => s.Items))
            .ForMember(d => d.GrandTotal,
                o => o.MapFrom(s =>
                    s.Items
                        .Where(i => i.Product.IsActive)
                        .Sum(i => i.Product.Price * i.Quantity)
                ));
    }
}
