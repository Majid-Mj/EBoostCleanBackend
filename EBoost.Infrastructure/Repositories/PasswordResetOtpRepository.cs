using EBoost.Domain.Entities;
using EBoost.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBoost.Application.Interfaces.Repositories;

namespace EBoost.Infrastructure.Repositories;

public class PasswordResetOtpRepository : IPasswordResetOtpRepository
{
    public readonly EBoostDbContext _context;

    public PasswordResetOtpRepository(EBoostDbContext context)
    {
        _context = context; 
    }

    public async Task AddAsync(PasswordResetOtp otp)
       => await _context.PasswordResetOtps.AddAsync(otp);

    public async Task<PasswordResetOtp?> GetLatestValidOtpAsync(string email)
      => await _context.PasswordResetOtps
          .Where(x => x.Email == email && !x.IsUsed)
          .OrderByDescending(x => x.CreatedAt)
          .FirstOrDefaultAsync();

    public async Task RemoveExistingOtpsAsync(string email)
    {
        var existing = _context.PasswordResetOtps
            .Where(x => x.Email == email);

        _context.PasswordResetOtps.RemoveRange(existing);
        await Task.CompletedTask;
    }


    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();   
}
