using EBoost.Domain.Entities;

namespace EBoost.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<List<RefreshToken>> GetAllValidAsync();
    Task RevokeAsync(int userId);

}
