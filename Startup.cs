using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDM.Models;
using App.Services;
using App.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IDM
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            
            services.Configure<GMailSettings>(Configuration.GetSection(nameof(GMailSettings)));
            services.AddSingleton<IEmailService, GMailService>();

            services.AddDbContext<IDMdbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("IDMdbContext")));

            services.AddIdentity<ColaboradorModel, IdentityRole<int>>(options =>
                {
                    options.User.RequireUniqueEmail = true; //false
                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; //idem
                    options.Password.RequireNonAlphanumeric = false; //true
                    options.Password.RequireUppercase = false; //true;
                    options.Password.RequireLowercase = false; //true;
                    options.Password.RequireDigit = false; //true;
                    options.Password.RequiredUniqueChars = 1; //1;
                    options.Password.RequiredLength = 6; //6;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3); //5
                    options.Lockout.MaxFailedAccessAttempts = 5; //5
                    options.Lockout.AllowedForNewUsers = true; //true		
                    options.SignIn.RequireConfirmedEmail = false; //false
                    options.SignIn.RequireConfirmedPhoneNumber = false; //false
                    options.SignIn.RequireConfirmedAccount = false; //false
                })
                .AddEntityFrameworkStores<IDMdbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "IDMRequest"; //AspNetCore.Cookies
                options.Cookie.HttpOnly = true; //true
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5); //14 dias
                options.LoginPath = "/Usuario/Login"; // /Account/Login
                options.LogoutPath = "/Home/Index";  // /Account/Logout
                options.AccessDeniedPath = "/Usuario/AcessoRestrito"; // /Account/AccessDenied
                options.SlidingExpiration = true; //true - gera um novo cookie a cada requisição se o cookie estiver com menos de meia vida
                options.ReturnUrlParameter = "returnUrl"; //returnUrl
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
        UserManager<ColaboradorModel> userManager, RoleManager<IdentityRole<int>> roleManager, IDMdbContext _context)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            Inicializador.InicializarIdentity(userManager, roleManager, _context);
        }
    }
}
