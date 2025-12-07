using KAShop.Dal.DTOs.Request;
using KAShop.Dal.DTOs.Response;
using KAShop.Dal.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KAShop.Bll.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginRequest.Email);
                if (user is null)
                {
                    return new LoginResponse()
                    {
                        Success = false,
                        Message = "invalid email",
                    };

                }
                var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
                if (!result)
                {
                    return new LoginResponse()
                    {
                        Success = false,
                        Message = "invalid password",
                    };
                }
                return new LoginResponse()
                {
                    Success = true,
                    Message = "login successfully",
                    AccessToken = await GenerateAccessToken(user)
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse()
                {
                    Success = false,
                    Message = "An Unexpected error",
                    Errors = new List<string> { ex.Message }
                };
            }

        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                var user = registerRequest.Adapt<ApplicationUser>();
                var result = await _userManager.CreateAsync(user, registerRequest.Password);
                if (!result.Succeeded)
                {
                    return new RegisterResponse()
                    {
                        Success = false,
                        Message = "User Creation Failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }
                await _userManager.AddToRoleAsync(user, "User");
                return new RegisterResponse()
                {
                    Success = true,
                    Message = "success"
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse()
                {
                    Success = false,
                    Message = "AnUnexpected error",
                    Errors = new List<string> { ex.Message }
                };

            }

        }
    
        private async Task<string> GenerateAccessToken(ApplicationUser user)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email),

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issure"],
                audience: _configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}
