
using KAShop.Bll.Service;
using KAShop.Dal.Data;
using KAShop.Dal.Models;
using KAShop.Dal.Repository;
using KAShop.Dal.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace KAShop.Pl
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddLocalization(options => options.ResourcesPath = "");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
          
              builder.Services.AddAuthentication(opt =>
              {
                  opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                  opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
              }
              )
                .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issure"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "KASHOP API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter: Bearer {your token}"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });
            const string defaultCulture = "en";

            var supportedCultures = new[]
            {
               new CultureInfo(defaultCulture),
               new CultureInfo("ar")
            };

            builder.Services.Configure<RequestLocalizationOptions>(options => {
                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider
                {
                    QueryStringKey = "lang"
                });
            });
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<ISeedData,RoleSeedData>();
            builder.Services.AddScoped<ISeedData, UserSeedData>();

            var app = builder.Build();

            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

             
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                
               app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            using (var scop = app.Services.CreateScope())
            {
                var services = scop.ServiceProvider;
                var seeders= services.GetServices<ISeedData>();
                foreach (var seeder in seeders) {
                    await seeder.DataSeed();
                }
            }
            app.MapControllers();

            app.Run();
        }

        
    }
}
