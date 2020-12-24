using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace ReportingSystem.Web.Filters
{
    public class UserIdHeaderRequiredFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Request.Headers;

            var userIdHeaderName = "UserId";
            if (!headers.TryGetValue(userIdHeaderName, out StringValues userId))
            {
                context.Result = new BadRequestObjectResult($"{userIdHeaderName} header is missed.");
            }
            context.HttpContext.Items.Add(userIdHeaderName, userId);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
