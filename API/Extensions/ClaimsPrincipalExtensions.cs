using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;                  // related to UniqueName (user.UserName) in the TokenService
        }
      
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;        // related to NameId (user.Id) in the TokenService
            // don't forget that we already automatically in the background recieve a token back from the API, so from this recieved token we can actually understand if it is the actual user who he/she claims to be
        }

    }
}