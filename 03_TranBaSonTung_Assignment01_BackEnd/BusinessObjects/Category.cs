namespace BackEnd.BusinessObjects;

public partial class Category
{
    public short CategoryID { get; set; }
    public string CategoryName { get; set; } = null!;
    public string CategoryDesciption { get; set; } = null!;
    public short? ParentCategoryID { get; set; }
    public bool? IsActive { get; set; }

    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();
    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
