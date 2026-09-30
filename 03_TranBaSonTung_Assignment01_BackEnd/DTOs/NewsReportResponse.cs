namespace BackEnd.DTOs;

public class NewsReportResponse
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalArticles { get; set; }
    public int ActiveArticles { get; set; }
    public int InactiveArticles { get; set; }
    public List<ReportGroupItem> ByCategory { get; set; } = new();
    public List<ReportGroupItem> ByAuthor { get; set; } = new();
    public List<ReportArticleItem> Articles { get; set; } = new();
}
