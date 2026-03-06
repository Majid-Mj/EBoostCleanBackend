namespace EBoost.Application.DTOs.Products;

public record ProductDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    int CategoryId,
    string CategoryName,
    bool IsFeatured,
    bool IsActive,
    List<ProductImageDto> Images
);
