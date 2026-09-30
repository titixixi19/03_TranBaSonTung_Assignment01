using FrontEnd.Filters;
using FrontEnd.Models;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers;

[RoleAuthorize("Staff")]
public class ProfileController : Controller
{
    private readonly ApiClient _api;

    public ProfileController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var profile = await _api.GetAsync<AccountVM>("api/Profile");
        if (profile == null)
        {
            TempData["Error"] = "Profile not found.";
            return RedirectToAction("Index", "Home");
        }
        return View(profile);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var profile = await _api.GetAsync<AccountVM>("api/Profile");
        return PartialView("_ProfileForm", new ProfileFormVM
        {
            AccountName = profile?.AccountName ?? string.Empty,
            AccountEmail = profile?.AccountEmail ?? string.Empty
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProfileFormVM model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_ProfileForm", model);
        }

        var result = await _api.PutAsync("api/Profile", new
        {
            model.AccountName,
            model.AccountEmail,
            AccountPassword = string.IsNullOrWhiteSpace(model.AccountPassword) ? null : model.AccountPassword
        });
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return PartialView("_ProfileForm", model);
        }

        HttpContext.Session.SetString(SessionKeys.AccountName, model.AccountName);
        HttpContext.Session.SetString(SessionKeys.AccountEmail, model.AccountEmail);
        TempData["Success"] = result.Message;
        return Json(new { success = true });
    }
}
