using FrontEnd.Filters;
using FrontEnd.Models;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers;

[RoleAuthorize("Staff")]
public class TagsController : Controller
{
    private readonly ApiClient _api;

    public TagsController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? keyword)
    {
        var url = "odata/Tags?$orderby=TagID";
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = OData.Literal(keyword);
            url += "&$filter=" + OData.Escape($"contains(tolower(TagName),{k}) or contains(tolower(Note),{k})");
        }

        ViewBag.Keyword = keyword;
        return View(await _api.GetODataListAsync<TagVM>(url));
    }

    [HttpGet]
    public IActionResult Create() => PartialView("_TagForm", new TagFormVM());

    [HttpPost]
    public async Task<IActionResult> Create(TagFormVM model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_TagForm", model);
        }
        var result = await _api.PostAsync("api/Tag", new { model.TagName, model.Note });
        return HandleResult(result, model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var tag = await _api.GetAsync<TagVM>($"odata/Tags({id})");
        if (tag == null)
        {
            return Content("<div class='alert alert-danger'>Tag not found.</div>", "text/html");
        }
        return PartialView("_TagForm", new TagFormVM { TagID = tag.TagID, TagName = tag.TagName ?? string.Empty, Note = tag.Note });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(TagFormVM model)
    {
        if (!model.IsEdit)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return PartialView("_TagForm", model);
        }
        var result = await _api.PutAsync($"api/Tag/{model.TagID}", new { model.TagName, model.Note });
        return HandleResult(result, model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _api.DeleteAsync($"api/Tag/{id}");
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    private IActionResult HandleResult(ApiResult result, TagFormVM model)
    {
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return PartialView("_TagForm", model);
        }
        TempData["Success"] = result.Message;
        return Json(new { success = true });
    }
}
