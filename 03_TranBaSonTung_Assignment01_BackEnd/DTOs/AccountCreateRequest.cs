using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs;

public class AccountCreateRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string AccountName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(70)]
    public string AccountEmail { get; set; } = string.Empty;

    [Required, Range(1, 2, ErrorMessage = "Role must be 1 (Staff) or 2 (Lecturer).")]
    public int AccountRole { get; set; }

    [Required, StringLength(70, MinimumLength = 2)]
    public string AccountPassword { get; set; } = string.Empty;
}
