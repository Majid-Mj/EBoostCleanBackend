using EBoost.Application.DTOs.Auth;
using EBoost.Application.Interfaces.Repositories;
using EBoost.Application.Interfaces.Services;
using EBoost.Domain.Entities;
using EBoost.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EBoost.Application.DTOs.Auth;

namespace EBoost.Infrastructure.Identity;

public class PasswordResetService : IPasswordResetService
{
    private readonly EBoostDbContext _context;
    private readonly IPasswordResetOtpRepository _otpRepository;
    private readonly IEmailService _emailService;

    public PasswordResetService(
        EBoostDbContext context,
        IPasswordResetOtpRepository otpRepository,
        IEmailService emailService)
    {
        _context = context;
        _otpRepository = otpRepository;
        _emailService = emailService;
    }

    public async Task SendOtpAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLower();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

        if (user == null)
            return;

        // Generate a cryptographically secure 6-digit OTP
        var otp = System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

        await _otpRepository.RemoveExistingOtpsAsync(normalizedEmail);

        var otpEntity = new PasswordResetOtp
        {
            Email = normalizedEmail,
            OtpHash = BCrypt.Net.BCrypt.HashPassword(otp),
            ExpiryTime = DateTime.UtcNow.AddMinutes(10) // 10 minutes matches the email text
        };

        await _otpRepository.AddAsync(otpEntity);
        await _otpRepository.SaveChangesAsync();

        await _emailService.SendAsync(normalizedEmail,
           "EBoost Password Reset OTP",
           $"Your OTP is {otp}. It expires in 10 minutes.");
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLower();

        var otpRecord = await _otpRepository.GetLatestValidOtpAsync(normalizedEmail);

        if (otpRecord == null ||
            otpRecord.ExpiryTime < DateTime.UtcNow ||
            !BCrypt.Net.BCrypt.Verify(dto.Otp, otpRecord.OtpHash))
        {
            throw new Exception("Invalid or expired OTP");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

        if (user == null)
            throw new Exception("Invalid request");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        otpRecord.IsUsed = true;

        // Invalidate refresh tokens
        var tokens = _context.RefreshTokens
            .Where(t => t.UserId == user.Id);

        _context.RefreshTokens.RemoveRange(tokens);

        await _context.SaveChangesAsync();
    }
}   