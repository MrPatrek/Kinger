namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<Photo> Photos { get; set; } = new();        // "new()" is the same as "new List<Photo>()"

        // We commented this out because in order to use it, we need to create whole object with all properties, which we don't always need (optimization), so we used a different approach for this one ...
        // public int GetAge()         // "Get" prefix is important for AutoMapper! Because it will automatcally calculate our int "Age" (GetAge calculates Age) ...
        // {
        //     return DateOfBirth.CalculateAge();
        // }

        // Many-to-Many relationship
        public List<UserLike> LikedByUsers { get; set; }        // users who liked this user
        public List<UserLike> LikedUsers { get; set; }          // whom this user likes
    }
}