using EBoost.Application.Interfaces.Repositories;
using EBoost.Domain.Entities;
using EBoost.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace EBoost.Infrastructure.Repositories;

public class ShippingAddressRepository : IShippingAddressRepository
{
    private readonly EBoostDbContext _context;

    public ShippingAddressRepository (EBoostDbContext context)
    {
        _context = context; 
    }

    public async Task<List<ShippingAddress>> GetByUserIdAsync(int userId)
        => await _context.ShippingAddresses
               .Where(a => a.UserId == userId)
               .ToListAsync();

    public async Task<ShippingAddress?> GetDefaultByUserIdAsync(int userId)
        => await _context.ShippingAddresses
            .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);

    public async Task<ShippingAddress?> GetByIdAsync(int id)
        => await _context.ShippingAddresses
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task AddAsync(ShippingAddress address)
        => await _context.ShippingAddresses.AddAsync(address);

    public async Task DeleteAsync(ShippingAddress address)
    {
        _context.ShippingAddresses.Remove(address);
    }


    public async Task SaveChangesAsync()
        =>await _context.SaveChangesAsync();


}
