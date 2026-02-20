using EBoost.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



namespace EBoost.Application.Interfaces.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
