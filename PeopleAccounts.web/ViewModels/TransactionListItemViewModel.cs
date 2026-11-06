namespace PeopleAccounts.web.ViewModels;

public class TransactionListItemViewModel
{
    public int Code { get; set; }

    public DateTime TransactionDate { get; set; }

    public DateTime CaptureDate { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; } = string.Empty;
}