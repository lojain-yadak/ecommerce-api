using KAShop.Bll.Service;
using KAShop.Dal.Repository;
using KAShop.Dal.Utils;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace KAShop.Pl
{
    public static class AddConfiguration
    {
        public static void Config(IServiceCollection Services) {
            Services.AddScoped<ICategoryRepository, CategoryRepository>();
            Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<IAuthenticationService, AuthenticationService>();
            Services.AddScoped<ISeedData, RoleSeedData>();
            Services.AddScoped<ISeedData, UserSeedData>();
            Services.AddTransient<IEmailSender, EmailSender>();
     }
               
    }
}
