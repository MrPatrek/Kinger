using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required] public string KnownAs { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public DateOnly? DateOfBirth { get; set; }           // optional (?) because we want Required to work
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }

        [Required]
        // [StringLength(8, MinimumLength = 4)]                // this was required before the ASP.NET Identity was introduced, so no need to leave it here now
        public string Password { get; set; }
    }
}