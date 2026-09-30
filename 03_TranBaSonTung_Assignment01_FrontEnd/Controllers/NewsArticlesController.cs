using FrontEnd.Filters;
using FrontEnd.Models;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers;

[RoleAuthorize("Staff")]
public class NewsArticlesController : Controller
{
    private const string Expand = "$expand=Category($select=CategoryName),CreatedBy($select=AccountName),Tags";
    private readonly ApiClient _api;

    public NewsArticlesController(ApiClient api) => _api = api;

    private short CurrentAccountId =>
        short.TryParse(HttpContext.Session.GetString(SessionKeys.AccountId), out var id) ? id : (short)0;

    public async Task<IActionResult> Index(string? keyword, short? categoryId, bool? status)
    {
        return View(await SearchAsync(keyword, categoryId, status, createdBy: null));
    }

    // News history created by the logged-in staff
    public async Task<IActionResult> History(string? keyword, short? categoryId, bool? status)
    {
        return View(await SearchAsync(keyword, categoryId, status, createdBy: CurrentAccountId));
    }

    private async Task<NewsSearchVM> SearchAsync(string? keyword, short? categoryId, bool? status, short? createdBy)
    {
        var filters = new List<string>();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = OData.Literal(keyword);
            filters.Add($"(contains(tolower(NewsTitle),{k}) or contains(tolower(Headline),{k}) or contains(tolower(NewsContent),{k}))");
        }
        if (categoryId.HasValue) filters.Add($"CategoryID eq {categoryId}");
        if (status.HasValue) filters.Add($"NewsStatus eq {status.Value.ToString().ToLower()}");
        if (createdBy.HasValue) filters.Add($"CreatedByID eq {createdBy}");

        var url = $"odata/NewsArticles?{Expand}&$orderby=CreatedDate desc";
        if (filters.Count > 0) url += "&$filter=" + OData.Escape(string.Join(" and ", filters));

        return new NewsSearchVM
        {
            Keyword = keyword,
            CategoryId = categoryId,
            Status = status,
            Articles = await _api.GetODataListAsync<NewsArticleVM>(url),
            Categories = await _api.GetODataListAsync<CategoryVM>("odata/Categories?$orderby=CategoryName")
        };
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new NewsArticleFormVM();
        await PopulateAsync(model);
        return PartialView("_NewsArticleForm", model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewsArticleFormVM model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateAsync(model);
            return PartialView("_NewsArticleForm", model);
        }
        var result = await _api.PostAsync("api/NewsArticle", ToRequest(model));
        return await HandleResultAsync(result, model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var article = await _api.GetAsync<NewsArticleVM>($"odata/NewsArticles('{OData.Escape(id.Replace("'", "''"))}')?$expand=Tags");
        if (article == null)
        {
            return Content("<div class='alert alert-danger'>News article not found.</div>", "text/html");
        }
        var model = new NewsArticleFormVM
        {
            NewsArticleID = article.NewsArticleID,
            NewsTitle = article.NewsTitle ?? string.Empty,
            Headline = article.Headline,
            NewsContent = article.NewsContent ?? string.Empty,
            NewsSource = article.NewsSource,
            CategoryID = article.CategoryID,
            NewsStatus = article.NewsStatus ?? false,
            TagIds = article.Tags.Select(t => t.TagID).ToList()
        };
        await PopulateAsync(model);
        return PartialView("_NewsArticleForm", model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(NewsArticleFormVM model)
    {
        if (!model.IsEdit)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            await PopulateAsync(model);
            return PartialView("_NewsArticleForm", model);
        }
        var result = await _api.PutAsync($"api/NewsArticle/{Uri.EscapeDataString(model.NewsArticleID!)}", ToRequest(model));
        return await HandleResultAsync(result, model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id, string? returnAction)
    {
        var result = await _api.DeleteAsync($"api/NewsArticle/{Uri.EscapeDataString(id)}");
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(returnAction == nameof(History) ? nameof(History) : nameof(Index));
    }

    private static object ToRequest(NewsArticleFormVM model) => new
    {
        model.NewsTitle,
        model.Headline,
        model.NewsContent,
        model.NewsSource,
        model.CategoryID,
        model.NewsStatus,
        model.TagIds
    };

    private async Task PopulateAsync(NewsArticleFormVM model)
    {
        var categories = await _api.GetODataListAsync<CategoryVM>("odata/Categories?$orderby=CategoryName");
        // Only active categories can be chosen, but keep the current one when editing
        model.CategoryOptions = categories.Where(c => c.IsActive == true || c.CategoryID == model.CategoryID).ToList();
        model.TagOptions = await _api.GetODataListAsync<TagVM>("odata/Tags?$orderby=TagName");
    }

    private async Task<IActionResult> HandleResultAsync(ApiResult result, NewsArticleFormVM model)
    {
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await PopulateAsync(model);
            return PartialView("_NewsArticleForm", model);
        }
        TempData["Success"] = result.Message;
        return Json(new { success = true });
    }
}
