using PeopleAccounts.web.DTOs;

namespace PeopleAccounts.web.Services;

public interface IAuthService
{
    Task<AuthenticatedUser?> AuthenticateAsync(string username, string password);
}