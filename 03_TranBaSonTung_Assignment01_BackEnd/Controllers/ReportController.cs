using BackEnd.BusinessObjects;
using BackEnd.DTOs;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AccountRoles.AdminName)]
public class ReportController : ControllerBase
{
    private readonly INewsArticleRepository _repository;

    public ReportController(INewsArticleRepository repository) => _repository = repository;

    // GET api/Report?startDate=2024-01-01&endDate=2024-12-31
    [HttpGet]
    public ActionResult<NewsReportResponse> Get([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default)
        {
            return BadRequest(new { message = "StartDate and EndDate are required." });
        }
        if (startDate.Date > endDate.Date)
        {
            return BadRequest(new { message = "StartDate must be before or equal to EndDate." });
        }

        var from = startDate.Date;
        var to = endDate.Date.AddDays(1).AddTicks(-1);
        var articles = _repository.GetNewsArticlesByPeriod(from, to);

        var report = new NewsReportResponse
        {
            StartDate = from,
            EndDate = endDate.Date,
            TotalArticles = articles.Count,
            ActiveArticles = articles.Count(a => a.NewsStatus == true),
            InactiveArticles = articles.Count(a => a.NewsStatus != true),
            ByCategory = articles
                .GroupBy(a => a.Category?.CategoryName ?? "(none)")
                .Select(g => new ReportGroupItem { Name = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count).ThenBy(g => g.Name)
                .ToList(),
            ByAuthor = articles
                .GroupBy(a => a.CreatedBy?.AccountName ?? "(unknown)")
                .Select(g => new ReportGroupItem { Name = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count).ThenBy(g => g.Name)
                .ToList(),
            Articles = articles
                .OrderByDescending(a => a.CreatedDate)
                .Select(a => new ReportArticleItem
                {
                    NewsArticleID = a.NewsArticleID,
                    NewsTitle = a.NewsTitle,
                    CategoryName = a.Category?.CategoryName,
                    AuthorName = a.CreatedBy?.AccountName,
                    NewsStatus = a.NewsStatus,
                    CreatedDate = a.CreatedDate
                })
                .ToList()
        };
        return Ok(report);
    }
}
