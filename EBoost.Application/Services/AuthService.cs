using EBoost.Application.DTOs.Auth;
using EBoost.Application.Interfaces.Repositories;
using EBoost.Application.Interfaces.Services;
using EBoost.Domain.Entities;
using System.Security.Claims;

public class AuthService
{
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _hasher;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private object _roleRepo;

    public AuthService(
        IUserRepository userRepo,
        ITokenService tokenService,
        IPasswordHasher hasher,
        IRefreshTokenRepository refreshTokenRepo,
        IRefreshTokenGenerator refreshTokenGenerator)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
        _hasher = hasher;
        _refreshTokenRepo = refreshTokenRepo;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task<AuthTokenResult?> Login(LoginDto dto)
    {
        var user = await _userRepo.GetByEmailAsync(dto.Email);
        if (user == null) return null;

        if (!_hasher.Verify(user.PasswordHash, dto.Password))
            return null;

        if (user.IsBlocked)
            throw new UnauthorizedAccessException("Your account is blocked.");

        var accessToken = _tokenService.CreateToken(user);
        var refreshToken = _refreshTokenGenerator.Generate();

        await _refreshTokenRepo.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        return new AuthTokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task Register(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        if (dto.Password != dto.ConfirmPassword)
            throw new ArgumentException("Passwords do not match.");

        var existingUser = await _userRepo.GetByEmailAsync(email);
        if (existingUser != null)
            throw new InvalidOperationException("User already exists.");

            

        var hashedPassword = _hasher.HashPassword(dto.Password);

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,  
            PasswordHash = hashedPassword,
            RoleId = 1
        };

        await _userRepo.AddAsync(user);
    }


    //refreshing the token
    public async Task<AuthTokenResult?> RefreshTokens(string refreshToken)
    {
        var validTokens = await _refreshTokenRepo.GetAllValidAsync();

        var storedToken = validTokens.FirstOrDefault(t =>
            BCrypt.Net.BCrypt.Verify(refreshToken, t.TokenHash));

        if (storedToken.User == null)
            return null;


        storedToken.IsRevoked = true;

        var user = storedToken.User;


        var newAccessToken = _tokenService.CreateToken(user);
        var newRefreshToken = _refreshTokenGenerator.Generate();

        var newRefreshEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepo.AddAsync(newRefreshEntity);

        return new AuthTokenResult
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    //Logout logic
    public async Task<bool> Logout(string refreshToken)
    {
        var tokens = await _refreshTokenRepo.GetAllValidAsync();

        var storedToken = tokens.FirstOrDefault(t =>
            BCrypt.Net.BCrypt.Verify(refreshToken, t.TokenHash));

        if (storedToken == null)
            return false;

        await _refreshTokenRepo.RevokeAsync(storedToken.UserId);

        return true;
    }


    //ProfileLogic
    public async Task<GetProfileDto?> GetProfile(ClaimsPrincipal user)
    {
        var userId = int.Parse(
            user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var entity = await _userRepo.GetByIdAsync(userId);
        if (entity == null)
            return null;

        return new GetProfileDto
        {
            Id = entity.Id,
            FullName = entity.FullName,
            Email = entity.Email,
            Role = entity.Role?.Name ?? "User",
            CreatedAt = entity.CreatedAt
        };

    }

    //Update ProfileLogic
    public async Task<bool> UpdateProfile(
    ClaimsPrincipal user,
    UpdateProfileDto dto)
    {
        var userId = int.Parse(
            user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var entity = await _userRepo.GetByIdAsync(userId);
        if (entity == null)
            return false;

        entity.FullName = dto.FullName ?? entity.FullName;

        await _userRepo.UpdateAsync(entity);
        return true;
    }
}
