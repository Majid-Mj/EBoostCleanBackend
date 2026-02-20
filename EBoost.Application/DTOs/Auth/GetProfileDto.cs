using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.DTOs.Auth;

public class GetProfileDto
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role {  get; set; } =string.Empty;
    public DateTime CreatedAt { get; set; }

}
