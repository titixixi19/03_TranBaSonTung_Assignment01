using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models;

public class NewsArticleFormVM
{
    public string? NewsArticleID { get; set; }
    public bool IsEdit => !string.IsNullOrEmpty(NewsArticleID);

    [Display(Name = "Title")]
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(400, MinimumLength = 2, ErrorMessage = "Title must be 2-400 characters.")]
    public string NewsTitle { get; set; } = string.Empty;

    [Display(Name = "Headline")]
    [Required(ErrorMessage = "Headline is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Headline must be 2-150 characters.")]
    public string Headline { get; set; } = string.Empty;

    [Display(Name = "Content")]
    [Required(ErrorMessage = "Content is required.")]
    [StringLength(4000, MinimumLength = 2, ErrorMessage = "Content must be 2-4000 characters.")]
    public string NewsContent { get; set; } = string.Empty;

    [Display(Name = "Source")]
    [StringLength(400, ErrorMessage = "Source must be at most 400 characters.")]
    public string? NewsSource { get; set; }

    [Display(Name = "Category")]
    [Required(ErrorMessage = "Please select a category.")]
    [Range(1, short.MaxValue, ErrorMessage = "Please select a category.")]
    public short? CategoryID { get; set; }

    [Display(Name = "Active")]
    public bool NewsStatus { get; set; } = true;

    public List<int> TagIds { get; set; } = new();

    public List<CategoryVM> CategoryOptions { get; set; } = new();
    public List<TagVM> TagOptions { get; set; } = new();
}
