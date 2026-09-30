using FrontEnd.Filters;
using FrontEnd.Models;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers;

[RoleAuthorize("Admin")]
public class ReportsController : Controller
{
    private readonly ApiClient _api;

    public ReportsController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(ReportFilterVM model)
    {
        // First visit: show the form with a default period and no validation errors
        if (!model.StartDate.HasValue && !model.EndDate.HasValue)
        {
            ModelState.Clear();
            model.StartDate = new DateTime(DateTime.Today.Year - 2, 1, 1);
            model.EndDate = DateTime.Today;
        }

        if (ModelState.IsValid)
        {
            var url = $"api/Report?startDate={model.StartDate:yyyy-MM-dd}&endDate={model.EndDate:yyyy-MM-dd}";
            model.Report = await _api.GetAsync<NewsReportVM>(url);
        }
        return View(model);
    }
}
