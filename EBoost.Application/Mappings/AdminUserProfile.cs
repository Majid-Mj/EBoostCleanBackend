using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBoost.Domain.Entities;
using EBoost.Application.DTOs.User;


namespace EBoost.Application.Mappings;

public class AdminUserProfile : Profile
{
    public AdminUserProfile()
    {
        CreateMap<User, AdminUserDto>()
            .ForMember(dest => dest.Role,
            opt => opt.MapFrom(src => src.Role.Name));
    }
}
