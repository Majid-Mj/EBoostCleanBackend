using EBoost.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.Interfaces.Repositories;
public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(bool includeInactive = false);
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task<ProductImage?> GetImageByIdAsync(int imageId);
    Task DeleteImageAsync(ProductImage image);
    Task SaveChangesAsync();
    Task<List<Product>> SearchAsync(string keyword);

    Task<(List<Product> Items, int TotalCount)> FilterAsync(
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        int page,
        int pageSize
    );

    Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
    int page,
    int pageSize
    );
    Task<bool> ExistsAsync(string name, int categoryId);
    Task<bool> ExistsAsync(string name, int categoryId, int excludeId);

}
