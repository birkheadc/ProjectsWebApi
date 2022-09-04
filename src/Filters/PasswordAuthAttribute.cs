using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProjectsWebApi.Filters;

public class PasswordAuthAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Refuse access if no password is included in request.
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var password))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Refuse access if password is included but is wrong.

        if (DoesPasswordMatch(password) == false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Allow access if password matches.
        await next();
    }

    private bool DoesPasswordMatch(string password)
    {
        string correct = Environment.GetEnvironmentVariable("ASPNETCORE_PASSWORD");
        if (correct is null || correct == password)
        {
            return true;
        }
        return false;
    }
}