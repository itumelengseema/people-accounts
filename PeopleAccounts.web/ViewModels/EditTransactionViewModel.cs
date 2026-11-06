using System.ComponentModel.DataAnnotations;

namespace PeopleAccounts.web.ViewModels;

public class EditTransactionViewModel : IValidatableObject
{
    [Range(1, int.MaxValue)]
    public int Code { get; set; }

    [Required(ErrorMessage = "Transaction date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Transaction date")]
    public DateTime? TransactionDate { get; set; }

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
                "Transaction date cannot be in the future.",
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