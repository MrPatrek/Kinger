using API.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class PresenceHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Others means everybody online except for us
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());     // send our username (who's logged in) to all other users
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            await base.OnDisconnectedAsync(exception);
        }
    }
}