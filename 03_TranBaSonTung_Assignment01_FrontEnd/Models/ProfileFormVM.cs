using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models;

public class ProfileFormVM
{
    [Display(Name = "Full name")]
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2-100 characters.")]
    public string AccountName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [StringLength(70)]
    public string AccountEmail { get; set; } = string.Empty;

    [Display(Name = "New password")]
    [StringLength(70, MinimumLength = 2, ErrorMessage = "Password must be 2-70 characters.")]
    [DataType(DataType.Password)]
    public string? AccountPassword { get; set; }

    [Display(Name = "Confirm new password")]
    [Compare(nameof(AccountPassword), ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }
}
