using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.Interfaces.Services;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool Verify(string hashedPassword, string password);
}
