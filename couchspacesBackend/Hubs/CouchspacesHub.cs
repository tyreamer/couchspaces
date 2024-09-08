using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace couchspacesBackend.Hubs
{
    public class CouchspacesHub : Hub
    {
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

        // Add more methods as needed for other components
    }
}
