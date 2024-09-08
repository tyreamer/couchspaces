using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
namespace couchspaces.Services
{
    public class SignalRService : IAsyncDisposable
    {
        private readonly NavigationManager _navigationManager;
        public HubConnection HubConnection { get; private set; }

        public SignalRService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            HubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("http://localhost:5050/couchspaceshub"))
                .Build();
        }

        public async Task StartConnectionAsync()
        {
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (HubConnection != null)
            {
                await HubConnection.DisposeAsync();
            }
        }
    }
}
