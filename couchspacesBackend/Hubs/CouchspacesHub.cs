using Microsoft.AspNetCore.SignalR;
using couchspacesShared.Models;
using couchspacesShared.Services;

namespace couchspacesBackend.Hubs
{
    public class CouchspacesHub : Hub
    {
        private readonly SpaceService _spaceService;

        public CouchspacesHub(SpaceService spaceService)
        {
            _spaceService = spaceService;
        }

        // Method for sending chat messages
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // Method for handling reactions
        public async Task SendReaction(string user, string reaction)
        {
            await Clients.All.SendAsync("ReceiveReaction", user, reaction);
        }

        // Method for handling media controls
        public async Task ControlMedia(string user, string command)
        {
            await Clients.All.SendAsync("MediaControl", user, command);
        }

        // Method for joining a space
        public async Task JoinSpace(string spaceId, User user)
        {
            await _spaceService.AddUserToSpace(spaceId, user);
            await Groups.AddToGroupAsync(Context.ConnectionId, spaceId);
            await Clients.Group(spaceId).SendAsync("UserJoined", user);
        }

        // Method for leaving a space
        public async Task LeaveSpace(string spaceId, User user)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, spaceId);
            await Clients.Group(spaceId).SendAsync("UserLeft", user);
        }

        // Method for setting user ready state
        public async Task SetUserReady(string spaceId, string userId, bool isReady)
        {
            _spaceService.SetUserReady(spaceId, userId, isReady);
            await Clients.Group(spaceId).SendAsync("UserReadyStateChanged", userId, isReady);
        }
    }
}
