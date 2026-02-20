using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.DTOs.Wishlist.cs;

public record WishlistDto(
    int WishlistId,
    List<WishlistItemDto> Items
);
