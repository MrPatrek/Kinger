namespace API.Entities
{
    public class Connection
    {
        public Connection()                 // empty constructor for EF so that it does not complain when creating migration, schema etc.
        {
            
        }

        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        public string ConnectionId { get; set; }            // It is a PK because of Connection (class name) + Id
        public string Username { get; set; }
    }
}