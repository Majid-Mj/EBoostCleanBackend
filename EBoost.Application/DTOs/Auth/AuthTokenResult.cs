namespace EBoost.Application.DTOs.Auth;

public class AuthTokenResult
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
