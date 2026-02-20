using EBoost.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.DTOs.Order;

public class CartCheckoutDto
{
    public int? AddressId { get; set; }

    public PaymentMethod PaymentMethod { get; set; }
}

