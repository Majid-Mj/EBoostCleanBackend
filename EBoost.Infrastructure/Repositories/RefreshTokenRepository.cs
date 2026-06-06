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

    /// <summary>Direct single-row lookup by SHA-256 hash — no BCrypt, instant.</summary>
    public async Task<RefreshToken?> GetByHashAsync(string sha256Hash)
    {
        return await _context.RefreshTokens
            .Include(r => r.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(r =>
                r.TokenHash == sha256Hash &&
                !r.IsRevoked &&
                r.ExpiresAt > DateTime.UtcNow);
    }

    /// <summary>Revokes exactly one token by its SHA-256 hash — no full-table scan.</summary>
    public async Task RevokeByHashAsync(string sha256Hash)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.TokenHash == sha256Hash && !r.IsRevoked);

        if (token != null)
        {
            token.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }
}
