namespace EBoost.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }


    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

}
