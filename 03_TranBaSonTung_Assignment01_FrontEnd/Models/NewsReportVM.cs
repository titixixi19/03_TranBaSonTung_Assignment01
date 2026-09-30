namespace FrontEnd.Models;

public class NewsReportVM
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalArticles { get; set; }
    public int ActiveArticles { get; set; }
    public int InactiveArticles { get; set; }
    public List<ReportGroupItemVM> ByCategory { get; set; } = new();
    public List<ReportGroupItemVM> ByAuthor { get; set; } = new();
    public List<ReportArticleItemVM> Articles { get; set; } = new();
}
