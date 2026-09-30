namespace FrontEnd.Models;

public class NewsArticleVM
{
    public string NewsArticleID { get; set; } = string.Empty;
    public string? NewsTitle { get; set; }
    public string Headline { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public string? NewsContent { get; set; }
    public string? NewsSource { get; set; }
    public short? CategoryID { get; set; }
    public bool? NewsStatus { get; set; }
    public short? CreatedByID { get; set; }
    public short? UpdatedByID { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public CategoryVM? Category { get; set; }
    public AccountVM? CreatedBy { get; set; }
    public List<TagVM> Tags { get; set; } = new();
}
