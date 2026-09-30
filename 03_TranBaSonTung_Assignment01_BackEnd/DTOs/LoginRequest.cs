using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs;

public class LoginRequest
{
    [Required, EmailAddress, StringLength(70)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(70)]
    public string Password { get; set; } = string.Empty;
}
