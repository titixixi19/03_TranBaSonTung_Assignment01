using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs;

public class ProfileUpdateRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string AccountName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(70)]
    public string AccountEmail { get; set; } = string.Empty;

    // Leave empty to keep the current password
    [StringLength(70, MinimumLength = 2)]
    public string? AccountPassword { get; set; }
}
