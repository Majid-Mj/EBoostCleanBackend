using EBoost.Domain.Entities;

namespace EBoost.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<List<RefreshToken>> GetAllValidAsync();
    Task RevokeAsync(int userId);

    /// <summary>Finds a single valid token by its SHA-256 hash. O(1) indexed lookup.</summary>
    Task<RefreshToken?> GetByHashAsync(string sha256Hash);

    /// <summary>Revokes a single specific token by its SHA-256 hash.</summary>
    Task RevokeByHashAsync(string sha256Hash);
}
