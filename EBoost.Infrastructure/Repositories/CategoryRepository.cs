using EBoost.Application.Interfaces.Repositories;
using EBoost.Domain.Entities;
using EBoost.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EBoost.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly EBoostDbContext _context;

    public CategoryRepository(EBoostDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
        => await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<bool> ExistsAsync(string name)
        => await _context.Categories.AnyAsync(c => c.Name.ToLower() == name.Trim().ToLower());
    public async Task<Category?> GetByIdAsync(int id)
    => await _context.Categories.FindAsync(id);

    public async Task AddAsync(Category category)
        => await _context.Categories.AddAsync(category);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
