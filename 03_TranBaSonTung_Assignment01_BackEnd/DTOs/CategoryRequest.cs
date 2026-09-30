using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs;

public class CategoryRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string CategoryName { get; set; } = string.Empty;

    [Required, StringLength(250, MinimumLength = 2)]
    public string CategoryDesciption { get; set; } = string.Empty;

    [Range(1, short.MaxValue)]
    public short? ParentCategoryID { get; set; }

    public bool IsActive { get; set; } = true;
}
