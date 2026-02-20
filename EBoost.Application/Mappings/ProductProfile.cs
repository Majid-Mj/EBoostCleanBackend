using AutoMapper;
using EBoost.Domain.Entities;
using EBoost.Application.DTOs.Products;

namespace EBoost.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Images, opt => opt.Ignore());

        CreateMap<UpdateProductDto, Product>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.Images, o => o.Ignore());

        // 👇 Map ProductImage → ProductImageDto
        CreateMap<ProductImage, ProductImageDto>();

        // 👇 Map Product → ProductDto
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName,
                o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.Images,
                o => o.MapFrom(s => s.Images));
    }
}
