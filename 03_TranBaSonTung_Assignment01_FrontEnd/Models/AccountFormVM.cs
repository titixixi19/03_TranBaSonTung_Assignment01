using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models;

public class AccountFormVM
{
    public short? AccountID { get; set; }
    public bool IsEdit => AccountID.HasValue;

    [Display(Name = "Full name")]
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2-100 characters.")]
    public string AccountName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [StringLength(70, ErrorMessage = "Email must be at most 70 characters.")]
    public string AccountEmail { get; set; } = string.Empty;

    [Display(Name = "Role")]
    [Required(ErrorMessage = "Role is required.")]
    [Range(1, 2, ErrorMessage = "Please select a valid role.")]
    public int AccountRole { get; set; } = 1;

    // Required on create; optional on edit (blank = keep current password)
    [Display(Name = "Password")]
    [StringLength(70, MinimumLength = 2, ErrorMessage = "Password must be 2-70 characters.")]
    [DataType(DataType.Password)]
    public string? AccountPassword { get; set; }
}
