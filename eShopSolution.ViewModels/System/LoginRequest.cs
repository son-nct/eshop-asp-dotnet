using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModels.System
{
    public class LoginRequest
    {
        public String UserName { get; set; }

        public String Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
