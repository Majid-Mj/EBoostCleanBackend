using EBoost.Application.Interfaces.Repositories;
using EBoost.Domain.Entities;
using EBoost.Domain.Enums;
using EBoost.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EBoost.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly EBoostDbContext _context;

    public OrderRepository(EBoostDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Order>> GetByUserIdAsync(int userId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    //for cancelOrder
    public async Task<Order?> GetByIdForUpdateAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }


    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await action();

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    //for get all Orders for Admin

    public async Task<(List<Order> Orders, int TotalCount)> GetAllAsync(
        int page,
        int pagesize,
        OrderStatus? status = null
        )
    {
        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .AsQueryable();

        if (status.HasValue )
        {
            query = query.Where(o => o.Status == status.Value);
        }

        var totalCount = await query.CountAsync();

        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pagesize)
            .Take(pagesize)
            .ToListAsync();

        return (orders, totalCount);
    }

}
