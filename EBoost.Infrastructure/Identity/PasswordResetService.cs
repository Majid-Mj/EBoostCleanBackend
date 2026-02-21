using EBoost.Application.DTOs.Auth;
using EBoost.Application.Interfaces.Repositories;
using EBoost.Application.Interfaces.Services;
using EBoost.Domain.Entities;
using EBoost.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return;

        var otp = new Random().Next(100000, 999999).ToString();

        await _otpRepository.RemoveExistingOtpsAsync(email);

        var otpEntity = new PasswordResetOtp
        {
            Email = email,
            OtpHash = BCrypt.Net.BCrypt.HashPassword(otp),
            ExpiryTime = DateTime.UtcNow.AddMinutes(10)
        };

        await _otpRepository.AddAsync(otpEntity);
        await _otpRepository.SaveChangesAsync();

        await _emailService.SendAsync(email,
           "EBoost Password Reset OTP",
           $"Your OTP is {otp}. It expires in 10 minutes.");
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var otpRecord = await _otpRepository.GetLatestValidOtpAsync(dto.Email);

        if (otpRecord == null ||
            otpRecord.ExpiryTime < DateTime.UtcNow ||
            !BCrypt.Net.BCrypt.Verify(dto.Otp, otpRecord.OtpHash))
        {
            throw new Exception("Invalid or expired OTP");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
            throw new Exception("Invalid request");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        otpRecord.IsUsed = true;

        //Invalidate refresh tokens
        var tokens =  _context.RefreshTokens
            .Where(t => t.UserId == user.Id);

        _context.RefreshTokens.RemoveRange(tokens);

        await _context.SaveChangesAsync();
    }
}   