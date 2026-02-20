using System.Security.Cryptography;
using EBoost.Application.Interfaces.Services;

namespace EBoost.Infrastructure.Identity;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public string Generate()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }
}
