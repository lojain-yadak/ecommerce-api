using KAShop.Dal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAShop.Dal.Utils
{
    public class UserSeedData : ISeedData
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserSeedData(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task DataSeed()
        {
            if (!await _userManager.Users.AnyAsync()) {
                var user1 = new ApplicationUser
                {
                    UserName = "lyadak",
                    Email = "l@gmail.com",
                    FullName = "lojain yadak",
                    EmailConfirmed = true,

                };
                var user2 = new ApplicationUser
                {
                    UserName = "Drabaya",
                    Email = "d@gmail.com",
                    FullName = "Duha Rabaya",
                    EmailConfirmed = true,

                };
                var user3 = new ApplicationUser
                {
                    UserName = "Abed",
                    Email = "a@gmail.com",
                    FullName = "Abed Edaily",
                    EmailConfirmed = true,

                };

                await _userManager.CreateAsync(user1,"Pass@1122");
                await _userManager.CreateAsync(user2,"Pass@1122");
                await _userManager.CreateAsync(user3,"Pass@1122");

                await _userManager.AddToRoleAsync(user1, "superAdmin");
                await _userManager.AddToRoleAsync(user2, "Admin");
                await _userManager.AddToRoleAsync(user3, "User");

            }

        }
    }
}
