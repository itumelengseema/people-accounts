using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PeopleAccounts.web.Data;
using PeopleAccounts.web.Models;
using PeopleAccounts.web.Services;

namespace PeopleAccounts.web;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        string connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddScoped<IPersonService, PersonService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<ITransactionService, TransactionService>();
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Auth/Login";

                options.Cookie.Name = "PeopleAccounts.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;

                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            });

        builder.Services.AddAuthorization();

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            using IServiceScope scope = app.Services.CreateScope();

            ApplicationDbContext context =
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            IPasswordHasher<User> passwordHasher =
                scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            await UserSeeder.SeedAsync(
                context,
                passwordHasher,
                app.Configuration);
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        await app.RunAsync();
    }
}