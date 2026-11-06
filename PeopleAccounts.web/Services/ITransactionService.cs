using PeopleAccounts.web.ViewModels;

namespace PeopleAccounts.web.Services;

public interface ITransactionService
{
    Task<CreateTransactionViewModel?> GetForCreateAsync(int accountCode);

    Task<CreateTransactionResult> CreateAsync(
        CreateTransactionViewModel model);
    
    Task<EditTransactionViewModel?> GetForEditAsync(int code);

    Task<UpdateTransactionResult> UpdateAsync(
        EditTransactionViewModel model);
}