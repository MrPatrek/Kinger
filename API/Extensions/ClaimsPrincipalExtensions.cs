using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;        // similar to NameId in the TokenService
            // don't forget that we already automatically in the background recieve a token back from the API, so from this recieved token we can actually understand if it is the actual user who he/she claims to be
        }
    }
}