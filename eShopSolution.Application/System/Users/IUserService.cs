using eShopSolution.ViewModels.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Users
{
    public interface IUserService
    {
        Task<String> Authenticate(LoginRequest request);

        Task<bool> Register(RegisterRequest request);
    }
}
