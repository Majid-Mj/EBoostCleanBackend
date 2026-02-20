using EBoost.Application.Interfaces.Repositories;
using EBoost.Domain.Entities;
using EBoost.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EBoost.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly EBoostDbContext _context;

    public UserRepository(EBoostDbContext context)
    {
        _context = context;
    }

    //public async Task<User?> GetByEmailAsync(string email)
    //    => await _context.Users
    //        .Include(u => u.Role)
    //        .FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
        => await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    //Get all User for Admin    
    public async Task<(List<User>, int)> GetAllAsync(int page, int pageSize)
    {
        var query = _context.Users.AsNoTracking();

        var totalCount = await query.CountAsync();

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    //toggle  active 
    public async Task<User?> GetByIdForUpdateAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id); 
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
