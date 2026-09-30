using BackEnd.BusinessObjects;

namespace BackEnd.Repositories;

public interface INewsArticleRepository
{
    List<NewsArticle> GetNewsArticles();
    NewsArticle? GetNewsArticleById(string id);
    List<NewsArticle> GetNewsArticlesByPeriod(DateTime startDate, DateTime endDate);
    NewsArticle AddNewsArticle(NewsArticle article, IEnumerable<int> tagIds);
    void UpdateNewsArticle(NewsArticle article, IEnumerable<int> tagIds);
    void DeleteNewsArticle(string id);
}
