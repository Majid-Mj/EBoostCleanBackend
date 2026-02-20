using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Domain.Entities;

public class ShippingAddress
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;

    public bool IsDefault { get; set; }

    public User User { get; set; } = null!;
}

