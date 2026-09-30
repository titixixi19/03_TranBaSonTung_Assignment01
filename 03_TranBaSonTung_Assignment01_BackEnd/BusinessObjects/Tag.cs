namespace BackEnd.BusinessObjects;

public partial class Tag
{
    public int TagID { get; set; }
    public string? TagName { get; set; }
    public string? Note { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
