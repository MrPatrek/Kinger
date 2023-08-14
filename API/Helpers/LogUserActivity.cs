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

            var userId = resultContext.HttpContext.User.GetUserId();

            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(userId);          // it is more optimal (efficient) to update user via userId and NOT userName because retrieving username also retrieves Photos, which makes our request a bit heavy, considering the fact that each such request will be executed each time we do smth in the client
            user.LastActive = DateTime.UtcNow;
            await repo.SaveAllAsync();
        }
    }
}