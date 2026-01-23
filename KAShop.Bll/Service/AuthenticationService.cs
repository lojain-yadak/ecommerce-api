using KAShop.Dal.DTOs.Request;
using KAShop.Dal.DTOs.Response;
using KAShop.Dal.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
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
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration,
            IEmailSender emailSender,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService
            )
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _tokenService = tokenService;
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
                if (await _userManager.IsLockedOutAsync(user)) {
                    return new LoginResponse()
                    {
                        Success=false,
                        Message= "Account is locked , try again  later"
                    };
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password,true);
                if (result.IsLockedOut)
                {
                    return new LoginResponse()
                    {
                        Success = false,
                        Message = "Account locked duo to multiple failed attempts"
                    };
                }
                else if (result.IsNotAllowed)
                {
                    return new LoginResponse()
                    {
                        Success = false,
                        Message = "plz confirm your email"
                    };
                }
                if (!result.Succeeded) {
                    return new LoginResponse()
                    {
                        Success = false,
                        Message = "invalid password"
                    };
                }
                var accessToken = await _tokenService.GenerateAccessToken(user);
                var refreshToken =  _tokenService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime=DateTime.UtcNow.AddDays(7);
               
                await _userManager.UpdateAsync(user);
                return new LoginResponse()
                {
                    Success = true,
                    Message = "login successfully",
                    AccessToken = accessToken,
                    RefreshToken=refreshToken
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
                var token =await _userManager.GenerateEmailConfirmationTokenAsync(user);
                token = Uri.EscapeDataString(token);
                var emailUrl = $"https://localhost:7127/api/auth/Account/ConfirmEmail?token={token}&userId={user.Id}";
                await _emailSender.SendEmailAsync(
                                  user.Email,
                                  "Welcome",
                                  $"<h1>Welcome...{user.UserName}</h1><a href='{emailUrl}'>Confirm Email</a>"
);
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
        public async Task<bool>ConfirmEmailAsync(string token,string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return false;
            return true;
        }
        
        public async Task<ForgetPasswordResponse> RequestPasswordReset(ForgetPasswordRequest request) {
        var user = await _userManager.FindByEmailAsync(request.Email);
            if(user is null)
            {
                return new ForgetPasswordResponse
                {
                    Success = false,
                    Message = "Email Not Found"
                };
            }
            var random = new Random();
            var code = random.Next(1000, 9999).ToString();
            user.CodeResetPassword = code;
            user.PasswordResetCodeExpiry=DateTime.UtcNow.AddMinutes(15);
            await _userManager.UpdateAsync(user);
            await _emailSender.SendEmailAsync(request.Email, "reset password", $"<p> your code to reset your password is {code}</p>");
            return new ForgetPasswordResponse()
            {
                Success=true,
                Message="code sent to your email"

            };
        }
        public async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request) {
        var user = await _userManager.FindByEmailAsync(request.Email);
            if(user is null)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Email Not Found"
                };
            }
            else if (user.CodeResetPassword != request.Code)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "invalid code"
                };
            }
            else if (user.PasswordResetCodeExpiry < DateTime.UtcNow)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "code expired"
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user,token,request.Password);
            if (!result.Succeeded)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Password reset failed",
                    Errors = result.Errors.Select(e=>e.Description).ToList()
                };
            }    
            
            await _emailSender.SendEmailAsync(request.Email, "change password", $"<p> your password changed</p>");

            user.CodeResetPassword = null;
            await _userManager.UpdateAsync(user);
            return new ResetPasswordResponse()
            {
                Success=true,
                Message="password reset successfully"

            };
        }
        public async Task<LoginResponse> RefreshTokenAsync(TokenApiModel request)
        {
            string accessToken = request.AccessToken;
            string refreshToken = request.RefreshToken;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var userName = principal.Identity.Name;
            var user = await _userManager.Users.FirstOrDefaultAsync(u=>u.UserName==userName);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new LoginResponse()
                {
                    Success = false,
                    Message = "invalid client request"
                };
            }
            var newAccessToken = await _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);
            return new LoginResponse()
            {
                Success = true,
                Message = "Token Refreshed",
                AccessToken= newAccessToken,
                RefreshToken=newRefreshToken,
            };
        }

    }


}
