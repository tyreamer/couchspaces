using Microsoft.AspNetCore.SignalR;
using couchspacesShared.Models;
using couchspacesShared.Services;
using StackExchange.Redis;

namespace couchspacesBackend.Hubs
{
    public class CouchspacesHub : Hub
    {
        private readonly SpaceService _spaceService;
        private readonly IDatabase _redisDatabase;

        public CouchspacesHub(SpaceService spaceService, IConnectionMultiplexer redis)
        {
            _spaceService = spaceService;
            _redisDatabase = redis.GetDatabase();
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendReaction(string user, string reaction)
        {
            await Clients.All.SendAsync("ReceiveReaction", user, reaction);
        }

        public async Task ControlMedia(string user, string command)
        {
            await Clients.All.SendAsync("MediaControl", user, command);
        }

        public async Task JoinSpace(string spaceId, User user)
        {
            await _spaceService.AddUserToSpace(spaceId, user);
            await Groups.AddToGroupAsync(Context.ConnectionId, spaceId);

            await _redisDatabase.SetAddAsync($"spaceUsers:{spaceId}", Context.ConnectionId);

            await Clients.Group(spaceId).SendAsync("UserJoined", user);
            await UpdateUserCounts(spaceId);
        }

        public async Task LeaveSpace(string spaceId, User user)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, spaceId);

            await _redisDatabase.SetRemoveAsync($"spaceUsers:{spaceId}", Context.ConnectionId);
            await _redisDatabase.SetRemoveAsync($"readyUsers:{spaceId}", Context.ConnectionId);

            await Clients.Group(spaceId).SendAsync("UserLeft", user);
            await UpdateUserCounts(spaceId);
        }

        public async Task UserReady(string spaceId, bool isReady)
        {
            if (isReady)
            {
                await _redisDatabase.SetAddAsync($"readyUsers:{spaceId}", Context.ConnectionId);
            }
            else
            {
                await _redisDatabase.SetRemoveAsync($"readyUsers:{spaceId}", Context.ConnectionId);
            }

            await UpdateUserCounts(spaceId);
        }

        private async Task UpdateUserCounts(string spaceId)
        {
            var totalUserCount = await _redisDatabase.SetLengthAsync($"spaceUsers:{spaceId}");
            var readyUserCount = await _redisDatabase.SetLengthAsync($"readyUsers:{spaceId}");

            await Clients.Group(spaceId).SendAsync("UpdateReadyUserCount", readyUserCount);
            await Clients.Group(spaceId).SendAsync("UpdateTotalUserCount", totalUserCount);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
