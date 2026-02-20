using EBoost.Application.DTOs.User;
using EBoost.Application.Interfaces.Repositories;
using EBoost.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;


namespace EBoost.Application.Services;

public class UserService :  IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepo, IMapper mapper)
    {
        _userRepo = userRepo;
        _mapper = mapper;
    }

    public async Task<(List<AdminUserDto>, int)> GetAllUsersAsync(
       int page,
       int pageSize)
    {
        var (users, totalCount) =
            await _userRepo.GetAllAsync(page, pageSize);

        var mapped = _mapper.Map<List<AdminUserDto>>(users);

        return (mapped, totalCount);
    }

    public async Task<AdminUserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);

        if (user == null)
            return null;

        return _mapper.Map<AdminUserDto>(user);
    }

    public async Task ToggleBlockUserAsync(int id, int adminId)
    {
        var user = await _userRepo.GetByIdForUpdateAsync(id);

        if (user == null)
            throw new Exception("User not found");

        if (user.Id == adminId)
            throw new Exception("Admin cannot block himself");

        if (user.RoleId == 2)
            throw new Exception("Cannot block another admin");

        user.IsBlocked = !user.IsBlocked;

        await _userRepo.SaveChangesAsync();
    }


}
