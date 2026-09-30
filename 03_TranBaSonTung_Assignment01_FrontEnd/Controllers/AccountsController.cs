using FrontEnd.Filters;
using FrontEnd.Models;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers;

[RoleAuthorize("Admin")]
public class AccountsController : Controller
{
    private readonly ApiClient _api;

    public AccountsController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? keyword, int? role)
    {
        var filters = new List<string>();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = OData.Literal(keyword);
            filters.Add($"(contains(tolower(AccountName),{k}) or contains(tolower(AccountEmail),{k}))");
        }
        if (role.HasValue)
        {
            filters.Add($"AccountRole eq {role}");
        }

        var url = "odata/SystemAccounts?$orderby=AccountID";
        if (filters.Count > 0) url += "&$filter=" + OData.Escape(string.Join(" and ", filters));

        ViewBag.Keyword = keyword;
        ViewBag.Role = role;
        return View(await _api.GetODataListAsync<AccountVM>(url));
    }

    [HttpGet]
    public IActionResult Create() => PartialView("_AccountForm", new AccountFormVM());

    [HttpPost]
    public async Task<IActionResult> Create(AccountFormVM model)
    {
        if (string.IsNullOrWhiteSpace(model.AccountPassword))
        {
            ModelState.AddModelError(nameof(model.AccountPassword), "Password is required.");
        }
        if (!ModelState.IsValid)
        {
            return PartialView("_AccountForm", model);
        }

        var result = await _api.PostAsync("api/Account", new
        {
            model.AccountName, model.AccountEmail, model.AccountRole, model.AccountPassword
        });
        return HandleResult(result, model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(short id)
    {
        var account = await _api.GetAsync<AccountVM>($"odata/SystemAccounts({id})");
        if (account == null)
        {
            return Content("<div class='alert alert-danger'>Account not found.</div>", "text/html");
        }
        return PartialView("_AccountForm", new AccountFormVM
        {
            AccountID = account.AccountID,
            AccountName = account.AccountName ?? string.Empty,
            AccountEmail = account.AccountEmail ?? string.Empty,
            AccountRole = account.AccountRole ?? 1
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(AccountFormVM model)
    {
        if (!model.IsEdit)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return PartialView("_AccountForm", model);
        }

        var result = await _api.PutAsync($"api/Account/{model.AccountID}", new
        {
            model.AccountName,
            model.AccountEmail,
            model.AccountRole,
            AccountPassword = string.IsNullOrWhiteSpace(model.AccountPassword) ? null : model.AccountPassword
        });
        return HandleResult(result, model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(short id)
    {
        var result = await _api.DeleteAsync($"api/Account/{id}");
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    private IActionResult HandleResult(ApiResult result, AccountFormVM model)
    {
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return PartialView("_AccountForm", model);
        }
        TempData["Success"] = result.Message;
        return Json(new { success = true });
    }
}
