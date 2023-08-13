namespace API.Entities
{
    public class UserLike
    {
        // Many-to-Many relationship
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }
        public AppUser TargetUser { get; set; }
        public int TargetUserId { get; set; }
    }
}