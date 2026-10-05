using System.Net;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FrontEnd.Filters;

// Handles errors coming from the Web API:
// - token rejected: clear the session and send the user back to login
// - API down / unexpected error: show a friendly message instead of the exception page
public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ITempDataDictionaryFactory _tempDataFactory;
    private readonly IModelMetadataProvider _metadataProvider;

    public ApiExceptionFilter(ITempDataDictionaryFactory tempDataFactory, IModelMetadataProvider metadataProvider)
    {
        _tempDataFactory = tempDataFactory;
        _metadataProvider = metadataProvider;
    }

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ApiUnauthorizedException ex:
                context.HttpContext.Session.Clear();
                _tempDataFactory.GetTempData(context.HttpContext)["Error"] = ex.Message;
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                break;

            case ApiException ex when IsAjax(context.HttpContext.Request):
                // Popup dialogs render the returned HTML inside the modal
                context.Result = new ContentResult
                {
                    Content = $"<div class='alert alert-danger mb-0'>{WebUtility.HtmlEncode(ex.Message)}</div>",
                    ContentType = "text/html"
                };
                break;

            case ApiException ex:
                context.Result = new ViewResult
                {
                    ViewName = "ApiError",
                    StatusCode = StatusCodes.Status503ServiceUnavailable,
                    ViewData = new ViewDataDictionary(_metadataProvider, context.ModelState) { Model = ex.Message }
                };
                break;

            default:
                return;
        }
        context.ExceptionHandled = true;
    }

    private static bool IsAjax(HttpRequest request) =>
        request.Headers.XRequestedWith == "XMLHttpRequest";
}
