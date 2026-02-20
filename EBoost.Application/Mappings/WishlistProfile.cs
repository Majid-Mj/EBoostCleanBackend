using AutoMapper;
using EBoost.Application.DTOs.Wishlist;
using EBoost.Application.DTOs.Wishlist.cs;
using EBoost.Domain.Entities;

namespace EBoost.Application.Mappings;

public class WishlistProfile : Profile
{
    public WishlistProfile()
    {
        CreateMap<WishlistItem, WishlistItemDto>()
            .ForMember(d => d.ProductName,
                o => o.MapFrom(s => s.Product.Name))
            .ForMember(d => d.Price,
                o => o.MapFrom(s => s.Product.Price))
            .ForMember(d => d.ImageUrl,
                o => o.MapFrom(s =>
                    s.Product.Images
                        .Where(i => i.IsPrimary)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
                ));
    }
}
