using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppRole : IdentityRole<int>            // same as in AppUser, we want int id-s, not the string ones
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}