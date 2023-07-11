using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")]       // if now this, then we would have had "Photo" name for table
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }

        public int AppUserId { get; set; }          // this line is in combination with the next one
        public AppUser AppUser { get; set; }        // we denote that our photo cannot exist without a user (i.e., AppUserId CANNOT be null)
                                                    // also, if the User is deleted, then his photos are deleted as well (cascade behaviour)
    }
}