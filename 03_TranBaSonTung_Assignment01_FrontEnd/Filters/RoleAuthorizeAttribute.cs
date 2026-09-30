using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FrontEnd.Filters;

// Restricts an action/controller to the given session roles (Admin / Staff)
public class RoleAuthorizeAttribute : ActionFilterAttribute
{
    private readonly string[] _roles;

    public RoleAuthorizeAttribute(params string[] roles) => _roles = roles;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var role = session.GetString(SessionKeys.Role);

        if (string.IsNullOrEmpty(session.GetString(SessionKeys.Token)))
        {
            var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl });
            return;
        }

        if (_roles.Length > 0 && !_roles.Contains(role))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
        }
    }
}
