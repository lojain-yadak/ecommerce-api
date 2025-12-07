using KAShop.Dal.DTOs.Request;
using KAShop.Dal.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAShop.Bll.Service
{
    public interface IAuthenticationService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest);
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
    }
}
