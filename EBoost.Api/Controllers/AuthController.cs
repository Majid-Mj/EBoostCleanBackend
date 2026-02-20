using EBoost.Application.DTOs.Auth;
using EBoost.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace EBoost.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{

    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm]RegisterDto dto)
    {
        await _authService.Register(dto);
        return Ok("User registered successfully");
    }
    

    [HttpPost("login")]
     public async Task<IActionResult> Login([FromForm]LoginDto dto)
    {
        var result = await _authService.Login(dto);

        if (result == null)
            return Unauthorized("Invalid email or password");

        
        Response.Cookies.Append("access_token", result.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15)
        });

        Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return Ok(new
        {
            accessToken = result.AccessToken,
            refreshToken = result.RefreshToken
        });
     }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Refresh token missing");

            var result = await _authService.RefreshTokens(refreshToken);

            if (result == null)
                return Unauthorized("Invalid or expired refresh token");

            Response.Cookies.Append("access_token", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken
            });
        }

        //LogOut    Api
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
                return Ok(new { message = "Already logged out" });

            await _authService.Logout(refreshToken);

            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return Ok(new { message = "Logged out successfully" });
        }

        //GetProfile Data
        [Authorize]
        [HttpGet("get-Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _authService.GetProfile(User);
            if (profile == null)
                return NotFound();
            
            return Ok(profile);
        }


        //Update Profile
        [Authorize]
        [HttpPut("Update-Profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var success = await _authService.UpdateProfile(User, dto);
            if (!success)
                return NotFound();

            return Ok(new { message = "Profile updated successfully" });
        }



}
