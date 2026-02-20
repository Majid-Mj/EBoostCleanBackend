using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.DTOs.Order;

public record OrderDto(
    int Id,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status,
     string ShippingFullName,
    string ShippingPhone,
    string ShippingStreet,
    string ShippingCity,
    string ShippingState,
    string ShippingPostalCode,
    string ShippingCountry,

    List<OrderItemDto> Items
);
