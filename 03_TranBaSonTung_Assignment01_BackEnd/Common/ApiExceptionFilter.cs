using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BackEnd.Common;

// Translates business-rule exceptions from the DAO layer into HTTP responses.
// Any other exception is left unhandled so internal errors are not exposed as validation messages.
public class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            NotFoundException ex => new NotFoundObjectResult(new { message = ex.Message }),
            BusinessException ex => new BadRequestObjectResult(new { message = ex.Message }),
            _ => null
        };
        context.ExceptionHandled = context.Result != null;
    }
}
