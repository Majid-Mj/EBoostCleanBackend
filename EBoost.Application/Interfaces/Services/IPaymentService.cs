using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.Interfaces.Services;

public interface IPaymentService
{
    Task<string> CreateCheckoutSessionAsync(int orderId);
    Task<bool> VerifyPaymentAsync(string sessionId);
}


