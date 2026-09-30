using BackEnd.BusinessObjects;
using BackEnd.Common;
using BackEnd.DTOs;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AccountRoles.StaffName)]
public class NewsArticleController : ControllerBase
{
    private readonly INewsArticleRepository _repository;

    public NewsArticleController(INewsArticleRepository repository) => _repository = repository;

    [HttpPost]
    public IActionResult Create([FromBody] NewsArticleRequest request)
    {
        var now = DateTime.Now;
        var userId = User.GetAccountId();
        var article = _repository.AddNewsArticle(new NewsArticle
        {
            NewsTitle = request.NewsTitle.Trim(),
            Headline = request.Headline.Trim(),
            NewsContent = request.NewsContent,
            NewsSource = request.NewsSource?.Trim(),
            CategoryID = request.CategoryID,
            NewsStatus = request.NewsStatus,
            CreatedByID = userId,
            UpdatedByID = userId,
            CreatedDate = now,
            ModifiedDate = now
        }, request.TagIds);
        return Ok(new { article.NewsArticleID, message = "News article created successfully." });
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] NewsArticleRequest request)
    {
        _repository.UpdateNewsArticle(new NewsArticle
        {
            NewsArticleID = id,
            NewsTitle = request.NewsTitle.Trim(),
            Headline = request.Headline.Trim(),
            NewsContent = request.NewsContent,
            NewsSource = request.NewsSource?.Trim(),
            CategoryID = request.CategoryID,
            NewsStatus = request.NewsStatus,
            UpdatedByID = User.GetAccountId(),
            ModifiedDate = DateTime.Now
        }, request.TagIds);
        return Ok(new { message = "News article updated successfully." });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        _repository.DeleteNewsArticle(id);
        return Ok(new { message = "News article deleted successfully." });
    }
}
