using couchspacesShared.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

namespace couchspacesShared.Services
{
    public class SpaceService
    {
        private readonly HttpClient _httpClient;
        private readonly HubConnection _hubConnection;

        public SpaceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5050/couchspaceshub")
                .Build();

            _hubConnection.On<User>("UserJoined", (user) =>
            {
                // Handle user joined event
            });

            _hubConnection.On<User>("UserLeft", (user) =>
            {
                // Handle user left event
            });

            _hubConnection.On<string, bool>("UserReadyStateChanged", (userId, isReady) =>
            {
                // Handle user ready state change
            });

            _hubConnection.On<bool>("AllUsersReadyStateChanged", (allReady) =>
            {
                // Handle all users ready state change
            });
        }

        public async Task StartAsync()
        {
            await _hubConnection.StartAsync();
        }

        public async Task<Space> CreateSpace(string name, string contentTitle, string streamingPlatform, ContentType spaceType, string streamingLink)
        {
            var space = new Space
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                ContentTitle = contentTitle,
                Users = new List<User>(),
                StreamingLink = streamingLink,
                StreamingPlatform = streamingPlatform,
                ContentType = spaceType
            };

            var response = await _httpClient.PostAsJsonAsync("api/space", space);
            response.EnsureSuccessStatusCode();

            var createdSpace = await response.Content.ReadFromJsonAsync<Space>();
            if (createdSpace == null)
            {
                throw new InvalidOperationException("Failed to create space. The response content is null.");
            }

            return createdSpace;
        }

        public async Task<Space?> GetSpace(string id)
        {
            return await _httpClient.GetFromJsonAsync<Space>($"api/space/{id}");
        }

        public async Task<bool> SpaceExists(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/space/exists/{id}");
        }

        public async Task AddUserToSpace(string spaceId, User user)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/space/{spaceId}/users", user);
            response.EnsureSuccessStatusCode();
        }

        public async Task JoinSpace(string spaceId, User user)
        {
            await _hubConnection.InvokeAsync("JoinSpace", spaceId, user);
        }

        public async Task LeaveSpace(string spaceId, User user)
        {
            await _hubConnection.InvokeAsync("LeaveSpace", spaceId, user);
        }

        public async Task<List<User>> GetUsersInSpace(string spaceId)
        {
            var users = await _httpClient.GetFromJsonAsync<List<User>>($"api/space/{spaceId}/users");
            return users ?? new List<User>();
        }

        public async Task SetUserReady(string spaceId, string userId, bool isReady)
        {
            await _hubConnection.InvokeAsync("SetUserReady", spaceId, userId, isReady);
        }

        public async Task CheckAllUsersReady(string spaceId)
        {
            await _hubConnection.InvokeAsync("CheckAllUsersReady", spaceId);
        }
    }
}

