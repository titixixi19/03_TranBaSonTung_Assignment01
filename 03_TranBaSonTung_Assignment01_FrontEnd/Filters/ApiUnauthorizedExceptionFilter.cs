using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FrontEnd.Filters;

// When the API rejects the token, clear the session and send the user back to login
public class ApiUnauthorizedExceptionFilter : IExceptionFilter
{
    private readonly ITempDataDictionaryFactory _tempDataFactory;

    public ApiUnauthorizedExceptionFilter(ITempDataDictionaryFactory tempDataFactory) => _tempDataFactory = tempDataFactory;

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ApiUnauthorizedException ex) return;

        context.HttpContext.Session.Clear();
        _tempDataFactory.GetTempData(context.HttpContext)["Error"] = ex.Message;
        context.Result = new RedirectToActionResult("Login", "Auth", null);
        context.ExceptionHandled = true;
    }
}
