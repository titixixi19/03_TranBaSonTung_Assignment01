using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BackEnd.Common;

// Translates business-rule exceptions from the DAO layer into HTTP responses
public class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            KeyNotFoundException ex => new NotFoundObjectResult(new { message = ex.Message }),
            InvalidOperationException ex => new BadRequestObjectResult(new { message = ex.Message }),
            _ => null
        };
        context.ExceptionHandled = context.Result != null;
    }
}
