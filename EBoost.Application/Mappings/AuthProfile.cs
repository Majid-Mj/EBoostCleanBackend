using AutoMapper;
using EBoost.Application.DTOs.Auth;
using EBoost.Domain.Entities;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<RegisterDto, User>()
            .ForMember(
                dest => dest.PasswordHash,
                opt => opt.Ignore() 
            );
    }
}   
