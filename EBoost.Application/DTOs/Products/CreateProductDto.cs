using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EBoost.Application.DTOs.Products;

public class CreateProductDto
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

    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, MinimumLength = 5,
    ErrorMessage = "Description must be between 5 and 500 characters")]
    [RegularExpression(@"^(?=.*[a-zA-Z0-9]).+$",
    ErrorMessage = "Description must contain at least one letter or number")]
    public string Description { get; set; } = null!;


    [Required(ErrorMessage = "Category is required")]
    [Range(1, int.MaxValue,
        ErrorMessage = "Invalid category")]
    public int CategoryId { get; set; }

    [Range(0, 100000,
        ErrorMessage = "Stock cannot be negative")]
    public int Stock { get; set; }

    public bool IsFeatured { get; set; }

    [Required(ErrorMessage = "At least one product image is required")]
    [MinLength(1, ErrorMessage = "At least one image must be uploaded")]
    public List<IFormFile> ImageFiles { get; set; } = new();

}
