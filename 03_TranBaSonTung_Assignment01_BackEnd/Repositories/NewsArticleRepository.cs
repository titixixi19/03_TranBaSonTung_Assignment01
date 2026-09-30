using BackEnd.BusinessObjects;
using BackEnd.DataAccess;

namespace BackEnd.Repositories;

public class NewsArticleRepository : INewsArticleRepository
{
    public List<NewsArticle> GetNewsArticles() => NewsArticleDAO.Instance.GetNewsArticles();
    public NewsArticle? GetNewsArticleById(string id) => NewsArticleDAO.Instance.GetNewsArticleById(id);
    public List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate) =>
        NewsArticleDAO.Instance.GetNewsArticlesByPeriod(startDate, endDate);
    public NewsArticle AddNewsArticle(NewsArticle article, IEnumerable<int> tagIds) =>
        NewsArticleDAO.Instance.AddNewsArticle(article, tagIds);
    public void UpdateNewsArticle(NewsArticle article, IEnumerable<int> tagIds) =>
        NewsArticleDAO.Instance.UpdateNewsArticle(article, tagIds);
    public void DeleteNewsArticle(string id) => NewsArticleDAO.Instance.DeleteNewsArticle(id);
}
