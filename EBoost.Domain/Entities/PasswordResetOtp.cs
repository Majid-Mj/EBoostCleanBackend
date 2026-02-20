using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Domain.Entities;

public  class PasswordResetOtp
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string OtpHash { get; set; }
    public DateTime ExpiryTime { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
