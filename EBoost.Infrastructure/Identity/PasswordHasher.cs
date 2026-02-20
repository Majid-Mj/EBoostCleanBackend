using EBoost.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using EBoost.Application.Interfaces;

namespace EBoost.Infrastructure.Identity;

public class PasswordHasherService : IPasswordHasher
{
    //private readonly PasswordHasher<object> _hasher = new();

    public string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string hashedPassword, string password)
        => BCrypt.Net.BCrypt.Verify(password, hashedPassword);

    //public string HashPassword(string password)
    //    => _hasher.HashPassword(null!, password);

    //public bool Verify(string hashedPassword, string password)
    //    => _hasher.VerifyHashedPassword(null!, hashedPassword, password)
    //       == PasswordVerificationResult.Success;
}
