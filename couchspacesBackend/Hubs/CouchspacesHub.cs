using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace couchspacesBackend.Hubs
{
    public class CouchspacesHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
