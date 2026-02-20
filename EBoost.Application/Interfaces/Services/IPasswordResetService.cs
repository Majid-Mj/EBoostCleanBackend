using EBoost.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.Interfaces.Services;

public interface IPasswordResetService
{
    Task SendOtpAsync(string email);
    Task ResetPasswordAsync(ResetPasswordDto dto);
}
