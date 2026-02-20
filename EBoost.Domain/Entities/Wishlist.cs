using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Domain.Entities;

public class Wishlist
{
    public int Id {get; set;}
    public int UserId {get; set;}

    public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
}
