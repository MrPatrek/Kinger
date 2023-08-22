using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    // // represents our Join Table between our Users and our Roles
    public class AppUserRole : IdentityUserRole<int>            // same as in AppUser, we want int id-s, not the string ones
    {
        public AppUser User { get; set; }       // UserId is inherited
        public AppRole Role { get; set; }       // RoleId       -||-
    }
}