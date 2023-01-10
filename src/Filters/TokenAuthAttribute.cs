using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectsWebApi.Services;

namespace ProjectsWebApi.Filters;

public class TokenAuthAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Refuse access if no authorization head is included in request.
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
        {
            
            context.Result = new UnauthorizedResult();
            return;
        }

        // Refuse access if token is included but is not valid.
        ISessionService sessionService = context.HttpContext.RequestServices.GetRequiredService<ISessionService>();
        if (sessionService.IsTokenValid(token) == false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Allow access.
        await next();
    }
}