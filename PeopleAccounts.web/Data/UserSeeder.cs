using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PeopleAccounts.web.Models;

namespace PeopleAccounts.web.Data;

public static class UserSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        string? username = configuration["SeedUser:Username"];
        string? password = configuration["SeedUser:Password"];

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrEmpty(password))
        {
            return;
        }

        string trimmedUsername = username.Trim();

        bool userExists = await context.Users
            .AnyAsync(user => user.Username == trimmedUsername);

        if (userExists)
        {
            return;
        }

        if (trimmedUsername.Length > 100 || password.Length < 12)
        {
            throw new InvalidOperationException(
                "The seed username must be at most 100 characters " +
                "and its password must be at least 12 characters.");
        }

        User user = new User
        {
            Username = trimmedUsername
        };

        user.PasswordHash = passwordHasher.HashPassword(user, password);

        context.Users.Add(user);

        await context.SaveChangesAsync();
    }
}