namespace PeopleAccounts.web.ViewModels;

public class DeletePersonViewModel
{
    public int Code { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string IdNumber { get; set; } = string.Empty;

    public int AccountCount { get; set; }

    public bool HasNonClosedAccounts { get; set; }

    public bool CanDelete => !HasNonClosedAccounts;
}