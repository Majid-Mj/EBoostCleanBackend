//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EBoost.Application.DTOs.Categories;

//public record UpdateCategoryDto(string Name);


using System.ComponentModel.DataAnnotations;

public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Category name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-&]+$",
        ErrorMessage = "Category name contains invalid characters")]
    public string Name { get; set; } = null!;
}
