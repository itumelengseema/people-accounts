namespace PeopleAccounts.web.ViewModels;

public class AccountListItemViewModel
{
    public int Code { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public decimal OutstandingBalance { get; set; }

    public string StatusName { get; set; } = string.Empty;
}