using System.Security.Claims;

namespace EBoost.Api.Extensions;

public static class ClaimsExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaims = user.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaims == null)
            throw new UnauthorizedAccessException("UserId claim not Found");

        return int.Parse(userIdClaims.Value);
    }

    public static int GetRoleId(this ClaimsPrincipal user)
    {
        var roleIdClaims = user.FindFirst("roleId");

        if (roleIdClaims == null)
            throw new UnauthorizedAccessException("Reload claim not found");

        return int.Parse(roleIdClaims.Value);
    }
}
