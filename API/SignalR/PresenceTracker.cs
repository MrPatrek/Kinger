namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

        public Task<bool> UserConnected(string username, string connectionId)
        {
            bool isOnline = false;          // a better naming for 'isOnline' var would be 'isFirstLogIn' or 'ifFirstSession'

            lock(OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string>{connectionId});
                    isOnline = true;
                }
            }

            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            bool isOffline = false;             // a better naming for 'isOffline' var would be 'noSessionsLeftForUser'

            lock(OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);          // IDK how we can reach this line IF USERS NEEDS TO BE ALREADY LOGGED IN for this method to be called, but this is done for safety

                OnlineUsers[username].Remove(connectionId);

                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline = true;
                }
            }

            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock(OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        public static Task<List<string>> GetConnectionsForUser(string username)             // check if user is online at all
        {
            List<string> connectionIds;

            lock(OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionIds);              // return all current connectionId-s for specified user (e.g., user tom with one current connection for smartphone, one for laptop, etc.)
        }
    }
}