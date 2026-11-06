using System.ComponentModel.DataAnnotations;

namespace PeopleAccounts.web.ViewModels;

public class CreateAccountViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "A valid person is required.")]
    public int PersonCode { get; set; }

    [Required(ErrorMessage = "Account number is required.")]
    [StringLength(50, ErrorMessage = "Account number cannot exceed 50 characters.")]
    [Display(Name = "Account number")]
    public string AccountNumber { get; set; } = string.Empty;
}