using Microsoft.AspNetCore.SignalR.Client;
using couchspacesShared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace couchspacesShared.Services
{
    public class SignalRService : IAsyncDisposable
    {
        public HubConnection HubConnection { get; private set; }
        public HubConnectionState ConnectionState => HubConnection.State;

        public event Action<int>? OnReadyUserCountUpdated;
        public event Action<int>? OnTotalUserCountUpdated;
        public event Action<string>? OnConnectionStatusChanged;
        public event Action<string, string>? OnMessageReceived;
        public event Action<string, string>? OnReactionReceived;
        public event Action<List<Message>>? OnLoadMessages;

        public SignalRService(string hubUrl)
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl) // Use the provided hub URL
                .Build();

            HubConnection.On<User>("UserJoined", (user) =>
            {
                // Handle user joined event
            });

            HubConnection.On<User>("UserLeft", (user) =>
            {
                // Handle user left event
            });

            HubConnection.On<string, bool>("UserReadyStateChanged", (userId, isReady) =>
            {
                // Handle user ready state change
            });

            HubConnection.On<bool>("AllUsersReadyStateChanged", (allReady) =>
            {
                // Handle all users ready state change
            });

            HubConnection.On<int>("UpdateReadyUserCount", (count) =>
            {
                OnReadyUserCountUpdated?.Invoke(count);
            });

            HubConnection.On<int>("UpdateTotalUserCount", (count) =>
            {
                OnTotalUserCountUpdated?.Invoke(count);
            });

            HubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                OnMessageReceived?.Invoke(user, message);
            });

            HubConnection.On<string, string>("ReceiveReaction", (user, reaction) =>
            {
                OnReactionReceived?.Invoke(user, reaction);
            });

            HubConnection.On<List<Message>>("LoadMessages", (messages) =>
            {
                OnLoadMessages?.Invoke(messages);
            });

            HubConnection.Closed += async (error) =>
            {
                OnConnectionStatusChanged?.Invoke("Disconnected");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await StartConnectionAsync();
            };

            HubConnection.Reconnected += (connectionId) =>
            {
                OnConnectionStatusChanged?.Invoke("Connected");
                return Task.CompletedTask;
            };

            HubConnection.Reconnecting += (error) =>
            {
                OnConnectionStatusChanged?.Invoke("Reconnecting");
                return Task.CompletedTask;
            };
        }

        public async Task StartConnectionAsync()
        {
            try
            {
                await HubConnection.StartAsync();
                OnConnectionStatusChanged?.Invoke("Connected");
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged?.Invoke("Disconnected");
                Console.WriteLine($"Error starting connection: {ex.Message}");
            }
        }

        public async Task EnsureConnectionStartedAsync()
        {
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await StartConnectionAsync();
            }
        }

        public async Task SendMessage(string spaceId, string user, string message)
        {
            await HubConnection.SendAsync("SendMessage", spaceId, user, message);
        }

        public async Task SendReaction(string user, string reaction)
        {
            await HubConnection.SendAsync("SendReaction", user, reaction);
        }

        public async Task JoinSpace(string spaceId, User user)
        {
            await HubConnection.InvokeAsync("JoinSpace", spaceId, user);
        }

        public async Task LeaveSpace(string spaceId, User user)
        {
            await HubConnection.InvokeAsync("LeaveSpace", spaceId, user);
        }

        public async Task SetUserReady(string spaceId, string userId, bool isReady)
        {
            await HubConnection.InvokeAsync("SetUserReady", spaceId, userId, isReady);
        }

        public async Task CheckAllUsersReady(string spaceId)
        {
            await HubConnection.InvokeAsync("CheckAllUsersReady", spaceId);
        }

        public async ValueTask DisposeAsync()
        {
            if (HubConnection != null)
            {
                await HubConnection.DisposeAsync();
                GC.SuppressFinalize(this);
            }
        }
    }
}
