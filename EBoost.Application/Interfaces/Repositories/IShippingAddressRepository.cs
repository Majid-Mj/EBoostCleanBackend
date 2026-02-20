using EBoost.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.Interfaces.Repositories;

public interface IShippingAddressRepository
{
    Task<List<ShippingAddress>> GetByUserIdAsync(int userId);
    Task<ShippingAddress?> GetDefaultByUserIdAsync(int userId);
    Task AddAsync(ShippingAddress address);
    Task<ShippingAddress?> GetByIdAsync(int id);
    Task DeleteAsync(ShippingAddress address);
    Task SaveChangesAsync();
}
