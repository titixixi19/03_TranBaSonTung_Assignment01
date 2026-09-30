using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs;

public class NewsArticleRequest
{
    [Required, StringLength(400, MinimumLength = 2)]
    public string NewsTitle { get; set; } = string.Empty;

    [Required, StringLength(150, MinimumLength = 2)]
    public string Headline { get; set; } = string.Empty;

    [Required, StringLength(4000, MinimumLength = 2)]
    public string NewsContent { get; set; } = string.Empty;

    [StringLength(400)]
    public string? NewsSource { get; set; }

    [Required, Range(1, short.MaxValue, ErrorMessage = "Please select a category.")]
    public short CategoryID { get; set; }

    public bool NewsStatus { get; set; } = true;

    public List<int> TagIds { get; set; } = new();
}
