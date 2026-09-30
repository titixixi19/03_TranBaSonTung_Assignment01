namespace FrontEnd.Models;

public class ReportArticleItemVM
{
    public string NewsArticleID { get; set; } = string.Empty;
    public string? NewsTitle { get; set; }
    public string? CategoryName { get; set; }
    public string? AuthorName { get; set; }
    public bool? NewsStatus { get; set; }
    public DateTime? CreatedDate { get; set; }
}
