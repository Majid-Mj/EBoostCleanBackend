using EBoost.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> GetByIdAsync(int id);
    Task<List<Order>> GetByUserIdAsync(int userId);
    Task<Order?> GetByIdForUpdateAsync(int id);
    Task SaveChangesAsync();
    Task ExecuteInTransactionAsync(Func<Task> action);

    Task<(List<Order> Orders, int TotalCount)> GetAllAsync(
    int page,
    int pageSize,
    OrderStatus? status = null);

    Task<Order?> GetByRazorpayOrderIdAsync(string razorpayOrderId);




}

