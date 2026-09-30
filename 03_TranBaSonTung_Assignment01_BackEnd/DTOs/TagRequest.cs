using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs;

public class TagRequest
{
    [Required, StringLength(50, MinimumLength = 2)]
    public string TagName { get; set; } = string.Empty;

    [StringLength(400)]
    public string? Note { get; set; }
}
