using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProjectsWebApi.Filters;

public class PasswordAuthAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        Console.WriteLine("Authorizing...");
        // Refuse access if no password is included in request.
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var password))
        {
            Console.WriteLine("Password not found; password must be included in 'Authorization' header.");
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
            Console.WriteLine("Password is correct.");
            return true;
        }
        Console.WriteLine("Password is NOT correct.");
        return false;
    }
}