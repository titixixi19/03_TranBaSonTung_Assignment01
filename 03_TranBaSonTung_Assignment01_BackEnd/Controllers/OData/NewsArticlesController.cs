using BackEnd.BusinessObjects;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BackEnd.Controllers.OData;

public class NewsArticlesController : ODataController
{
    private readonly INewsArticleRepository _repository;

    public NewsArticlesController(INewsArticleRepository repository) => _repository = repository;

    // Anonymous users only see active news; Staff/Admin see everything
    private bool CanSeeInactive => User.IsInRole(AccountRoles.StaffName) || User.IsInRole(AccountRoles.AdminName);

    // Public readers only get the author's name, never email/role
    private static NewsArticle HideAuthorDetails(NewsArticle article)
    {
        if (article.CreatedBy != null)
        {
            article.CreatedBy = new SystemAccount
            {
                AccountID = article.CreatedBy.AccountID,
                AccountName = article.CreatedBy.AccountName
            };
        }
        return article;
    }

    [EnableQuery(MaxExpansionDepth = 3)]
    public IActionResult Get()
    {
        var articles = _repository.GetNewsArticles();
        if (!CanSeeInactive)
        {
            articles = articles.Where(n => n.NewsStatus == true).Select(HideAuthorDetails).ToList();
        }
        return Ok(articles.AsQueryable());
    }

    [EnableQuery(MaxExpansionDepth = 3)]
    public IActionResult Get([FromRoute] string key)
    {
        var article = _repository.GetNewsArticleById(key);
        if (article == null || (!CanSeeInactive && article.NewsStatus != true))
        {
            return NotFound();
        }
        return Ok(CanSeeInactive ? article : HideAuthorDetails(article));
    }
}
