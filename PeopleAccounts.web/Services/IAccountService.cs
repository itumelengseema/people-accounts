using PeopleAccounts.web.ViewModels;

namespace PeopleAccounts.web.Services;

public interface IAccountService
{
    Task<CreateAccountViewModel?> GetForCreateAsync(int personCode);

    Task<CreateAccountResult> CreateAsync(
        CreateAccountViewModel model);
    
    Task<EditAccountViewModel?> GetForEditAsync(int code);

    Task<UpdateAccountResult> UpdateAsync(EditAccountViewModel model);
    
    Task<AccountDetailsViewModel?> GetDetailsAsync(
        int code,
        int pageNumber,
        int pageSize);
}