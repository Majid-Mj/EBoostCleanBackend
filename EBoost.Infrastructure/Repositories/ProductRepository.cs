using EBoost.Application.Interfaces.Repositories;
using EBoost.Domain.Entities;
using EBoost.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly EBoostDbContext _context;

    public ProductRepository(EBoostDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsQueryable();

        if (!includeInactive)
            query = query.Where(p => p.IsActive);

        return await query.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
        => await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Product product)
        => await _context.Products.AddAsync(product);

    //search
    public async Task<List<Product>> SearchAsync(string keyword)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive && p.Name.Contains(keyword))
            .ToListAsync();
    }

    //Filter
    public async Task<(List<Product>, int)> FilterAsync(
    int? categoryId,
    decimal? minPrice,
    decimal? maxPrice,
    int page,
    int pageSize)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    //pagination
    public async Task<(List<Product>, int)> GetPagedAsync(
    int page,
    int pageSize)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    //image delete
    public async Task<ProductImage?> GetImageByIdAsync(int imageId)
    {
        return await _context.ProductImages
            .FirstOrDefaultAsync(i => i.Id == imageId);
    }

    public async Task DeleteImageAsync(ProductImage image)
    {
        _context.ProductImages.Remove(image);
    }

    public async Task<bool> ExistsAsync(string name, int categoryId, int excludeId)
    {
        return await _context.Products
            .AnyAsync(p =>
                p.Id != excludeId &&
                p.CategoryId == categoryId &&
                p.Name.ToLower() == name.Trim().ToLower());
    }

    public async Task<bool> ExistsAsync(string name, int categoryId)
    {
        return await _context.Products
            .AnyAsync(p =>
                p.CategoryId == categoryId &&
                p.Name.ToLower() == name.Trim().ToLower());
    }

    //update version
    public async Task<Product?> GetByIdForUpdateAsync(int id)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);
    }



    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
