using EBoost.Application.Common.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace EBoost.Api.Extensions;

public static class AuthenticationExtensions
{
    //public static IServiceCollection AddJwtAuthentication(
    //    this IServiceCollection services,
    //    IConfiguration configuration)
    //{
    //    services.AddAuthentication(options =>
    //    {
    //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //    })
    //    .AddJwtBearer(options =>
    //    {
    //        options.TokenValidationParameters = new TokenValidationParameters
    //        {
    //            ValidateIssuer = true,
    //            ValidateAudience = true,
    //            ValidateLifetime = true,
    //            ValidateIssuerSigningKey = true,

    //            ValidIssuer = configuration["Jwt:Issuer"],
    //            ValidAudience = configuration["Jwt:Audience"],
    //            IssuerSigningKey = new SymmetricSecurityKey(
    //                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
    //            ),

    //            RoleClaimType = ClaimTypes.Role
    //        };
    public static IServiceCollection AddJwtAuthentication(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ClockSkew = TimeSpan.Zero,
                RoleClaimType = "roleId"
            };


            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["access_token"];
                    return Task.CompletedTask;
                },



                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    var response = ApiResponse<string>.Fail(
                        "Unauthorized",
                        StatusCodes.Status401Unauthorized
                    );

                    return context.Response.WriteAsJsonAsync(response);
                },

                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                    var response = ApiResponse<string>.Fail(
                        "Access denied",
                        StatusCodes.Status403Forbidden
                    );

                    return context.Response.WriteAsJsonAsync(response);
                }
            };

        });

        return services;
    }
}
