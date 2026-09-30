using System.Text.Json;
using FrontEnd.Models;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers;

public class AuthController : Controller
{
    private readonly ApiClient _api;

    public AuthController(ApiClient api) => _api = api;

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeys.Token)))
        {
            return RedirectToHome();
        }
        return View(new LoginVM { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _api.SendAsync(HttpMethod.Post, "api/Auth/login",
            new { model.Email, model.Password }, throwOnUnauthorized: false);
        if (!result.Success || result.Data == null)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        var data = result.Data.Value;
        string Get(string name) => data.TryGetProperty(name, out var v) ? v.ToString() : string.Empty;

        HttpContext.Session.SetString(SessionKeys.Token, Get("token"));
        HttpContext.Session.SetString(SessionKeys.AccountId, Get("accountID"));
        HttpContext.Session.SetString(SessionKeys.AccountName, Get("accountName"));
        HttpContext.Session.SetString(SessionKeys.AccountEmail, Get("accountEmail"));
        HttpContext.Session.SetString(SessionKeys.Role, Get("role"));

        TempData["Success"] = $"Welcome, {Get("accountName")}!";
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }
        return RedirectToHome();
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["Success"] = "You have been logged out.";
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();

    private IActionResult RedirectToHome() =>
        HttpContext.Session.GetString(SessionKeys.Role) switch
        {
            "Admin" => RedirectToAction("Index", "Accounts"),
            "Staff" => RedirectToAction("Index", "NewsArticles"),
            _ => RedirectToAction("Index", "Home")
        };
}
