//using EBoost.Application.Interfaces.Services;
//using EBoost.Domain.Entities;
//using Microsoft.AspNetCore.Authorization.Infrastructure;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Identity.Client;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace EBoost.Infrastructure.Identity;

//public class TokenService : ITokenService
//{
//    private readonly IConfiguration _config;

//    public TokenService(IConfiguration config)
//    {
//        _config = config;   
//    }

//    public string CreateToken(User user)
//    {

//        if (user.Role == null)
//            throw new Exception("User role not loaded");

//        var jwtKey = _config["Jwt:Key"];

//        if (string.IsNullOrWhiteSpace(jwtKey))
//            throw new Exception("JWT Key is missing in configuration");

//        var claims = new List<Claim>
//        {
//            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//            new Claim(ClaimTypes.Email, user.Email),
//            new Claim(ClaimTypes.Role, user.Role.Name) 
//        };

//        var key = new SymmetricSecurityKey(
//            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

//        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//        var token = new JwtSecurityToken(
//          issuer: _config["Jwt:Issuer"],
//          audience: _config["Jwt:Audience"],
//          claims: claims,
//          expires: DateTime.Now.AddHours(2),
//          signingCredentials: creds);

//        return new JwtSecurityTokenHandler().WriteToken(token);
//    }

//}



using EBoost.Application.Interfaces.Services;
using EBoost.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EBoost.Infrastructure.Identity;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateToken(User user)
    {
        //if (user.Role == null)
        //    throw new Exception("User role not loaded");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("roleId", user.RoleId.ToString())
            //new Claim(ClaimTypes.Role, user.Role.Name)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
