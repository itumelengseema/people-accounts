using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PeopleAccounts.web.Data;
using PeopleAccounts.web.DTOs;
using PeopleAccounts.web.Models;

namespace PeopleAccounts.web.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        ApplicationDbContext db,
        IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }
    public async Task<AuthenticatedUser?> AuthenticateAsync(
        string username,
        string password)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrEmpty(password))
        {
            return null;
        }

        string trimmedUsername = username.Trim();

        User? user = await _db.Users
            .SingleOrDefaultAsync(
                existing => existing.Username == trimmedUsername);

        if (user == null)
        {
            return null;
        }

        PasswordVerificationResult result =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash =
                _passwordHasher.HashPassword(user, password);

            await _db.SaveChangesAsync();
        }

        return new AuthenticatedUser
        {
            Code = user.Code,
            Username = user.Username
        };
    }
}