using BackEnd.BusinessObjects;
using BackEnd.Common;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.DataAccess;

public sealed class NewsArticleDAO
{
    private static NewsArticleDAO? instance;
    private static readonly object instanceLock = new();

    private NewsArticleDAO() { }

    public static NewsArticleDAO Instance
    {
        get
        {
            lock (instanceLock)
            {
                instance ??= new NewsArticleDAO();
                return instance;
            }
        }
    }

    private static IQueryable<NewsArticle> Query(FUNewsManagementContext context) =>
        context.NewsArticles
            .AsNoTracking()
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.Tags);

    public List<NewsArticle> GetNewsArticles()
    {
        using var context = new FUNewsManagementContext();
        return Query(context).OrderByDescending(n => n.CreatedDate).ToList();
    }

    public NewsArticle? GetNewsArticleById(string id)
    {
        using var context = new FUNewsManagementContext();
        return Query(context).FirstOrDefault(n => n.NewsArticleID == id);
    }

    public List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate)
    {
        using var context = new FUNewsManagementContext();
        return Query(context)
            .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
            .OrderByDescending(n => n.CreatedDate)
            .ToList();
    }

    private static string GenerateNewId(FUNewsManagementContext context)
    {
        var max = context.NewsArticles
            .Select(n => n.NewsArticleID)
            .AsEnumerable()
            .Select(id => int.TryParse(id, out var value) ? value : 0)
            .DefaultIfEmpty(0)
            .Max();
        return (max + 1).ToString();
    }

    // currentCategoryId: the article's category before an update. An article may keep a category
    // that became inactive later, but cannot be moved into (or created in) an inactive category.
    private static void ValidateReferences(FUNewsManagementContext context, NewsArticle article, short? currentCategoryId = null)
    {
        var category = context.Categories.AsNoTracking().FirstOrDefault(c => c.CategoryID == article.CategoryID)
                       ?? throw new BusinessException("Category does not exist.");

        if (category.IsActive != true && category.CategoryID != currentCategoryId)
            throw new BusinessException("The selected category is inactive.");
    }

    public NewsArticle AddNewsArticle(NewsArticle article, IEnumerable<int> tagIds)
    {
        using var context = new FUNewsManagementContext();
        ValidateReferences(context, article);

        article.NewsArticleID = GenerateNewId(context);
        var ids = tagIds.Distinct().ToList();
        article.Tags = context.Tags.Where(t => ids.Contains(t.TagID)).ToList();

        context.NewsArticles.Add(article);
        context.SaveChanges();
        return article;
    }

    public void UpdateNewsArticle(NewsArticle article, IEnumerable<int> tagIds)
    {
        using var context = new FUNewsManagementContext();
        var existing = context.NewsArticles.Include(n => n.Tags)
                           .FirstOrDefault(n => n.NewsArticleID == article.NewsArticleID)
                       ?? throw new NotFoundException("News article not found.");
        ValidateReferences(context, article, existing.CategoryID);

        existing.NewsTitle = article.NewsTitle;
        existing.Headline = article.Headline;
        existing.NewsContent = article.NewsContent;
        existing.NewsSource = article.NewsSource;
        existing.CategoryID = article.CategoryID;
        existing.NewsStatus = article.NewsStatus;
        existing.UpdatedByID = article.UpdatedByID;
        existing.ModifiedDate = article.ModifiedDate;

        var ids = tagIds.Distinct().ToList();
        existing.Tags.Clear();
        foreach (var tag in context.Tags.Where(t => ids.Contains(t.TagID)))
        {
            existing.Tags.Add(tag);
        }

        context.SaveChanges();
    }

    public void DeleteNewsArticle(string id)
    {
        using var context = new FUNewsManagementContext();
        var article = context.NewsArticles.Include(n => n.Tags).FirstOrDefault(n => n.NewsArticleID == id)
                      ?? throw new NotFoundException("News article not found.");

        // Remove NewsTag rows first (no cascade on that FK)
        article.Tags.Clear();
        context.NewsArticles.Remove(article);
        context.SaveChanges();
    }
}
