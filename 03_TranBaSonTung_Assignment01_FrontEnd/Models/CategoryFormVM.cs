using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models;

public class CategoryFormVM
{
    public short? CategoryID { get; set; }
    public bool IsEdit => CategoryID.HasValue;

    [Display(Name = "Category name")]
    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be 2-100 characters.")]
    public string CategoryName { get; set; } = string.Empty;

    [Display(Name = "Description")]
    [Required(ErrorMessage = "Description is required.")]
    [StringLength(250, MinimumLength = 2, ErrorMessage = "Description must be 2-250 characters.")]
    public string CategoryDesciption { get; set; } = string.Empty;

    [Display(Name = "Parent category")]
    public short? ParentCategoryID { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public List<CategoryVM> ParentOptions { get; set; } = new();
}
