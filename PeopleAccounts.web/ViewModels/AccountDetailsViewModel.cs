namespace PeopleAccounts.web.ViewModels;

public class AccountDetailsViewModel
{
    public int Code { get; set; }

    public int PersonCode { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public decimal OutstandingBalance { get; set; }

    public int StatusCode { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public PagedResult<TransactionListItemViewModel> Transactions
    {
        get;
        set;
    } = new PagedResult<TransactionListItemViewModel>();
}