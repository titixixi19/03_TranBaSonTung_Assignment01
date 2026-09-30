using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models;

public class TagFormVM
{
    public int? TagID { get; set; }
    public bool IsEdit => TagID.HasValue;

    [Display(Name = "Tag name")]
    [Required(ErrorMessage = "Tag name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Tag name must be 2-50 characters.")]
    public string TagName { get; set; } = string.Empty;

    [Display(Name = "Note")]
    [StringLength(400, ErrorMessage = "Note must be at most 400 characters.")]
    public string? Note { get; set; }
}
