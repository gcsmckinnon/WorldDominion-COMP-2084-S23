using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorldDominion.Data;
using WorldDominion.Models;

namespace WorldDominion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>() // Enables roles (not on by default)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Add dependency so controllers can read config values
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

            // Add Sessions
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllersWithViews();

            // Enable Google Auth
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    // Access Google Auth section of appsettings.Development.json
                    IConfigurationSection googleAuth = builder.Configuration.GetSection("Authentication:Google");

                    // Read Google API Key values from config
                    options.ClientId = googleAuth["ClientId"];
                    options.ClientSecret = googleAuth["ClientSecret"];
                });


            var app = builder.Build();

            // Use the session
            app.UseSession();
            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            RouteConfig.ConfigureRoutes(app);

            app.MapRazorPages();

            app.Run();
        }
    }
}