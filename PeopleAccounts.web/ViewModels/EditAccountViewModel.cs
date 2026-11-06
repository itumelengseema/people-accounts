using System.ComponentModel.DataAnnotations;

namespace PeopleAccounts.web.ViewModels;

public class EditAccountViewModel
{
    [Range(1, int.MaxValue)]
    public int Code { get; set; }

    [Required(ErrorMessage = "Account number is required.")]
    [StringLength(
        50,
        ErrorMessage = "Account number cannot exceed 50 characters.")]
    [Display(Name = "Account number")]
    public string AccountNumber { get; set; } = string.Empty;

    [Range(1, 2, ErrorMessage = "Select a valid account status.")]
    [Display(Name = "Status")]
    public int StatusCode { get; set; }
}