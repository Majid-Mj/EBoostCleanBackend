using EBoost.Application.Interfaces.Repositories;
using EBoost.Domain.Entities;
using EBoost.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EBoost.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly EBoostDbContext _context;

    public RefreshTokenRepository(EBoostDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<List<RefreshToken>> GetAllValidAsync()
    {
        return await _context.RefreshTokens
            .Include(r => r.User)
            .ThenInclude(u => u.Role)
            .Where(r => !r.IsRevoked && r.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task RevokeAsync(int userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked)
            .ToListAsync();

        foreach (var t in tokens)
            t.IsRevoked = true;

        await _context.SaveChangesAsync();
    }
}
