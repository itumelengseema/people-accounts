using System.ComponentModel.DataAnnotations;

namespace PeopleAccounts.web.ViewModels;

public class EditPersonViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Invalid person code.")]
    public int Code { get; set; }

    [StringLength(50, ErrorMessage =
        "Name cannot exceed 50 characters.")]
    public string? Name { get; set; }

    [StringLength(50, ErrorMessage =
        "Surname cannot exceed 50 characters.")]
    public string? Surname { get; set; }

    [Required(ErrorMessage = "ID number is required.")]
    [StringLength(50, ErrorMessage =
        "ID number cannot exceed 50 characters.")]
    [Display(Name = "ID number")]
    public string IdNumber { get; set; } = string.Empty;
}