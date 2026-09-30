using FrontEnd.Filters;
using FrontEnd.Models;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers;

[RoleAuthorize("Staff")]
public class CategoriesController : Controller
{
    private readonly ApiClient _api;

    public CategoriesController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? keyword, bool? status)
    {
        var filters = new List<string>();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = OData.Literal(keyword);
            filters.Add($"(contains(tolower(CategoryName),{k}) or contains(tolower(CategoryDesciption),{k}))");
        }
        if (status.HasValue)
        {
            filters.Add($"IsActive eq {status.Value.ToString().ToLower()}");
        }

        var url = "odata/Categories?$expand=ParentCategory($select=CategoryName)&$orderby=CategoryID";
        if (filters.Count > 0) url += "&$filter=" + OData.Escape(string.Join(" and ", filters));

        ViewBag.Keyword = keyword;
        ViewBag.Status = status;
        return View(await _api.GetODataListAsync<CategoryVM>(url));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CategoryFormVM();
        await PopulateAsync(model);
        return PartialView("_CategoryForm", model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryFormVM model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateAsync(model);
            return PartialView("_CategoryForm", model);
        }
        var result = await _api.PostAsync("api/Category", ToRequest(model));
        return await HandleResultAsync(result, model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(short id)
    {
        var category = await _api.GetAsync<CategoryVM>($"odata/Categories({id})");
        if (category == null)
        {
            return Content("<div class='alert alert-danger'>Category not found.</div>", "text/html");
        }
        var model = new CategoryFormVM
        {
            CategoryID = category.CategoryID,
            CategoryName = category.CategoryName,
            CategoryDesciption = category.CategoryDesciption,
            ParentCategoryID = category.ParentCategoryID,
            IsActive = category.IsActive ?? false
        };
        await PopulateAsync(model);
        return PartialView("_CategoryForm", model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CategoryFormVM model)
    {
        if (!model.IsEdit)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            await PopulateAsync(model);
            return PartialView("_CategoryForm", model);
        }
        var result = await _api.PutAsync($"api/Category/{model.CategoryID}", ToRequest(model));
        return await HandleResultAsync(result, model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(short id)
    {
        var result = await _api.DeleteAsync($"api/Category/{id}");
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    private static object ToRequest(CategoryFormVM model) => new
    {
        model.CategoryName,
        model.CategoryDesciption,
        model.ParentCategoryID,
        model.IsActive
    };

    private async Task PopulateAsync(CategoryFormVM model)
    {
        var all = await _api.GetODataListAsync<CategoryVM>("odata/Categories?$orderby=CategoryName");
        model.ParentOptions = all.Where(c => c.CategoryID != model.CategoryID).ToList();
    }

    private async Task<IActionResult> HandleResultAsync(ApiResult result, CategoryFormVM model)
    {
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await PopulateAsync(model);
            return PartialView("_CategoryForm", model);
        }
        TempData["Success"] = result.Message;
        return Json(new { success = true });
    }
}
