using System.Diagnostics;
using FrontEnd.Models;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers;

// Public news pages - no login required (the API only returns active news for anonymous users)
public class HomeController : Controller
{
    private const string Expand = "$expand=Category($select=CategoryName),CreatedBy($select=AccountName),Tags";
    private readonly ApiClient _api;

    public HomeController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? keyword, short? categoryId)
    {
        var filters = new List<string> { "NewsStatus eq true" };
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = OData.Literal(keyword);
            filters.Add($"(contains(tolower(NewsTitle),{k}) or contains(tolower(Headline),{k}) or contains(tolower(NewsContent),{k}))");
        }
        if (categoryId.HasValue)
        {
            filters.Add($"CategoryID eq {categoryId}");
        }

        var url = $"odata/NewsArticles?{Expand}&$orderby=CreatedDate desc&$filter={OData.Escape(string.Join(" and ", filters))}";
        var model = new NewsSearchVM
        {
            Keyword = keyword,
            CategoryId = categoryId,
            Articles = await _api.GetODataListAsync<NewsArticleVM>(url),
            Categories = await _api.GetODataListAsync<CategoryVM>("odata/Categories?$filter=IsActive eq true&$orderby=CategoryName")
        };
        return View(model);
    }

    public async Task<IActionResult> Details(string id)
    {
        var article = await _api.GetAsync<NewsArticleVM>($"odata/NewsArticles('{OData.Escape(id.Replace("'", "''"))}')?{Expand}");
        if (article == null)
        {
            TempData["Error"] = "News article not found.";
            return RedirectToAction(nameof(Index));
        }
        return View(article);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
