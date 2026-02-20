namespace EBoost.Application.DTOs.Products;

public record ProductDto(
    int Id,
    string Name,
    decimal Price,
    int Stock,
    int CategoryId,
    string CategoryName,
     List<ProductImageDto> Images
);
