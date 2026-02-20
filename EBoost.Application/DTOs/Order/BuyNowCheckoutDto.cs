using EBoost.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.DTOs.Order;

public class BuyNowCheckoutDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public int? AddressId { get; set; }

    public PaymentMethod PaymentMethod { get; set; }
}

