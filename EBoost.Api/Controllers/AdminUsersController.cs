using EBoost.Api.Extensions;
using EBoost.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EBoost.Api.Controllers;


[Authorize(Policy = "AdminOnly")]
[Route("api/admin/users")]
[ApiController]
public class AdminUsersController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminUsersController( IUserService userService)
    {
        _userService = userService; 
    }

    [HttpGet ("Users")]
    public async Task<IActionResult> GetAll(
       int page = 1,
       int pageSize = 10)
    {
        var (users, totalCount) =
            await _userService.GetAllUsersAsync(page, pageSize);

        return Ok(new
        {
            totalCount,
            page,
            pageSize,
            data = users
        });
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }


    [HttpPatch("{id:int}/toggle-block")]
    public async Task<IActionResult> ToggleBlock(int id)
    {
        int adminId = User.GetUserId();

        await _userService.ToggleBlockUserAsync(id, adminId);

        return Ok("User block status updated");
    }

}
