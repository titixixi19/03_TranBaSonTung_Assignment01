namespace FrontEnd.Models;

public class NewsSearchVM
{
    public string? Keyword { get; set; }
    public short? CategoryId { get; set; }
    public bool? Status { get; set; }
    public List<NewsArticleVM> Articles { get; set; } = new();
    public List<CategoryVM> Categories { get; set; } = new();
}
