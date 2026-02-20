using EBoost.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.Interfaces.Repositories;

public interface IPasswordResetOtpRepository
{
    Task AddAsync(PasswordResetOtp otp);
    Task<PasswordResetOtp?> GetLatestValidOtpAsync(string email);
    Task RemoveExistingOtpsAsync(string email);
    Task SaveChangesAsync();
}
