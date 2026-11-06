using PeopleAccounts.web.Models;
using PeopleAccounts.web.ViewModels;

namespace PeopleAccounts.web.Services;

public interface IPersonService
{
    Task<PagedResult<Person>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
    Task<bool> CreateAsync(CreatePersonViewModel model);
    Task<EditPersonViewModel?> GetForEditAsync(int code);
    Task<DeletePersonViewModel?> GetForDeleteAsync(int code);
    Task<UpdatePersonResult> UpdateAsync(EditPersonViewModel model);
    Task<DeletePersonResult> DeleteAsync(int code);
}