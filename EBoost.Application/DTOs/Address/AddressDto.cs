using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.DTOs.Address;

public record AddressDto(
    int Id,
    string FullName,
    string PhoneNumber,
    string Street,
    string City,
    string State,
    string PostalCode,
    string Country,
    bool IsDefault
);

