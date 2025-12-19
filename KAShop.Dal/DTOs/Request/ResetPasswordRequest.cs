using KAShop.Dal.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAShop.Dal.DTOs.Request
{
    public class ResetPasswordRequest
    {
        public string Code { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
