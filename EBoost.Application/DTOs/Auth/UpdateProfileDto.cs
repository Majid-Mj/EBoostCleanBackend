using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.DTOs.Auth;

public class UpdateProfileDto
{
    [MinLength(3, ErrorMessage = "Full name must be at least 3 characters")]
    [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$",
                ErrorMessage = "Full name can contain only letters and spaces")]
    public string? FullName { get; set; }
}
