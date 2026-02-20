using System.ComponentModel.DataAnnotations;

namespace EBoost.Application.DTOs.Products;

public class UpdateProductDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Product name must be between 3 and 100 characters")]
    [RegularExpression(@"^(?![\W_]+$)[a-zA-Z0-9\s\-_&()]+$",
        ErrorMessage = "Product name contains invalid characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Price is required")]
    [Range(typeof(decimal), "0.01", "99999999",
        ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Range(0, 100000,
        ErrorMessage = "Stock cannot be negative")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [Range(1, int.MaxValue,
        ErrorMessage = "Invalid category")]
    public int CategoryId { get; set; }
}
