using EBoost.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EBoost.Application.Interfaces.Repositories;

// Change from class to interface and remove method bodies
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(int id);
    Task UpdateAsync(User user);
    Task<(List<User>, int)> GetAllAsync(int page, int pageSize);

    Task<User?> GetByIdForUpdateAsync(int id);
    Task SaveChangesAsync();

}
