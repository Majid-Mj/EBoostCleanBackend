using EBoost.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.Interfaces.Services;

public interface IUserService
{
    Task<(List<AdminUserDto>, int)> GetAllUsersAsync(int page, int pageSize);

    Task<AdminUserDto?> GetUserByIdAsync(int id);

    Task ToggleBlockUserAsync(int id, int adminId);
}
