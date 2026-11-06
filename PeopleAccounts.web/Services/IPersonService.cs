using PeopleAccounts.web.Models;
using PeopleAccounts.web.ViewModels;

namespace PeopleAccounts.web.Services;

public interface IPersonService
{
    Task<PagedResult<Person>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
}