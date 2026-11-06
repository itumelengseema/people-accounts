namespace PeopleAccounts.web.ViewModels;

public class PersonDetailsViewModel
{
    public int Code { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string IdNumber { get; set; } = string.Empty;

    public PagedResult<AccountListItemViewModel> Accounts { get; set; }
        = new PagedResult<AccountListItemViewModel>();
}