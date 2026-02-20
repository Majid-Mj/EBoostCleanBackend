using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Domain.Entities;

public class WishlistItem
{
    public int Id { get; set; }
    public int WishlistId { get; set; }
    public  Wishlist Wishlist { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public DateTime CreatedAt { get; set; } =DateTime.UtcNow;
}
