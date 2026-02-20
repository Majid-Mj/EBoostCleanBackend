using System.ComponentModel.DataAnnotations;

public class UpdateProductStockDto
{
    [Required]
    [Range(0, 100000,
        ErrorMessage = "Stock must be between 0 and 100000")]
    public int Stock { get; set; }
}
