using Microsoft.AspNetCore.SignalR;
using couchspacesShared.Models;
using couchspacesShared.Services;
using StackExchange.Redis;
using System.Collections.Concurrent;
using couchspacesShared.Repositories;

namespace couchspacesBackend.Hubs
{
    public class CouchspacesHub : Hub
    {
        private readonly SpaceService _spaceService;
        private readonly MessageRepository _messageRepository;
        private static readonly ConcurrentDictionary<string, List<couchspacesShared.Models.Message>> _messages = new();

        public CouchspacesHub(SpaceService spaceService, MessageRepository messageRepository)
        {
            _spaceService = spaceService;
            _messageRepository = messageRepository;
        }

        public async Task SendMessage(string spaceId, string user, string message)
        {
            var newMessage = new couchspacesShared.Models.Message { User = user, Content = message };
            await _messageRepository.AddMessageAsync(spaceId, newMessage); // Use the repository to add the message
            await Clients.Group(spaceId).SendAsync("ReceiveMessage", user, message);
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

            await Clients.Group(spaceId).SendAsync("UserJoined", user);
            //await UpdateUserCounts(spaceId);

            // Load messages from the MessageRepository
            var messages = await _messageRepository.GetMessagesAsync(spaceId); // Use the repository to get messages
            await Clients.Caller.SendAsync("LoadMessages", messages);
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
