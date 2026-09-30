namespace BackEnd.BusinessObjects;

public partial class NewsArticle
{
    public string NewsArticleID { get; set; } = null!;
    public string? NewsTitle { get; set; }
    public string Headline { get; set; } = null!;
    public DateTime? CreatedDate { get; set; }
    public string? NewsContent { get; set; }
    public string? NewsSource { get; set; }
    public short? CategoryID { get; set; }
    public bool? NewsStatus { get; set; }
    public short? CreatedByID { get; set; }
    public short? UpdatedByID { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public virtual Category? Category { get; set; }
    public virtual SystemAccount? CreatedBy { get; set; }
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
