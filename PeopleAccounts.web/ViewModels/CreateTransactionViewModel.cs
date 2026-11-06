using System.ComponentModel.DataAnnotations;

namespace PeopleAccounts.web.ViewModels;

public class CreateTransactionViewModel : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "A valid account is required.")]
    public int AccountCode { get; set; }

    [Required(ErrorMessage = "Transaction date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Transaction date")]
    public DateTime? TransactionDate { get; set; } = DateTime.Today;

    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(
        100,
        ErrorMessage = "Description cannot exceed 100 characters.")]
    public string Description { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (TransactionDate.HasValue &&
            TransactionDate.Value.Date > DateTime.Today)
        {
            yield return new ValidationResult(
                "Transaction date cannot be future dated",
                new[] { nameof(TransactionDate) });
        }

        if (Amount == 0m)
        {
            yield return new ValidationResult(
                "Amount cannot be zero.",
                new[] { nameof(Amount) });
        }
    }
}