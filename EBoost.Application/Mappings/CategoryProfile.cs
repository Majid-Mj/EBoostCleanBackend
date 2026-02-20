using AutoMapper;
using EBoost.Domain.Entities;
using EBoost.Application.DTOs.Categories;

namespace EBoost.Application.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryDto, Category>();
        //CreateMap<UpdateCategoryDto, Category>();
    }
}
