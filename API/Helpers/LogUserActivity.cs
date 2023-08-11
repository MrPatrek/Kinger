using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // We can either make smth DURING the execution of some request, or AFTER the request has ended.
            // In this case, it doesn't really matter which one we choose,
            // so we will log the activity after some request has completed
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;           // this should already be the case since we can't reach up to here if we're not authenticated, by just to be sure we'll do this anyway

            var username = resultContext.HttpContext.User.GetUsername();

            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await repo.GetUserByUsernameAsync(username);
            user.LastActive = DateTime.UtcNow;
            await repo.SaveAllAsync();
        }
    }
}