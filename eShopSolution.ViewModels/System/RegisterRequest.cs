using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModels.System
{
    public class RegisterRequest
    {
        public String FirstName { get; set; }

        public String LastName { get; set; }

        public DateTime Dob { get; set; }

        public String Email { get; set; }

        public String PhoneNumber { get; set; }

        public String UserName { get; set; }

        public String Password { get; set; }

        public String ConfirmPassword { get; set; }
    }                                       
}
