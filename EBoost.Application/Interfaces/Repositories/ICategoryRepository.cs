using EBoost.Domain.Entities;

namespace EBoost.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(string name);
    Task AddAsync(Category category);
    Task SaveChangesAsync();
}
