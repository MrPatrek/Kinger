using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Group
    {
        public Group()          // empty constructor for EF so that it does not complain when creating migration, schema etc.
        {
            
        }

        public Group(string name)
        {
            Name = name;
        }

        [Key]
        public string Name { get; set; }
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}