using couchspacesShared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace couchspacesShared.Services
{
    public class SpaceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SignalRService _signalRService;

        public SpaceService(IHttpClientFactory httpClientFactory, SignalRService signalRService)
        {
            _httpClientFactory = httpClientFactory;
            _signalRService = signalRService;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient("BackendAPI");
            client.BaseAddress = new Uri("https://localhost:7160");
            return client;
        }

        public async Task<Space> CreateSpace(string contentTitle, string streamingPlatform, ContentType spaceType, string streamingLink)
        {
            var space = new Space
            {
                Id = Guid.NewGuid().ToString(),
                ContentTitle = contentTitle,
                Users = new List<User>(),
                StreamingLink = streamingLink,
                StreamingPlatform = streamingPlatform,
                ContentType = spaceType
            };

            var client = CreateClient();
            var response = await client.PostAsJsonAsync("api/space", space);
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
            var client = CreateClient();
            return await client.GetFromJsonAsync<Space>($"api/space/{id}");
        }

        public async Task<bool> SpaceExists(string id)
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<bool>($"api/space/exists/{id}");
        }

        public async Task AddUserToSpace(string spaceId, User user)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync($"api/space/{spaceId}/users", user);
            response.EnsureSuccessStatusCode();
        }

        public async Task JoinSpace(string spaceId, User user)
        {
            await _signalRService.JoinSpace(spaceId, user);
        }

        public async Task LeaveSpace(string spaceId, User user)
        {
            await _signalRService.LeaveSpace(spaceId, user);
        }

        public async Task<List<User>> GetUsersInSpace(string spaceId)
        {
            var client = CreateClient();
            var users = await client.GetFromJsonAsync<List<User>>($"api/space/{spaceId}/users");
            return users ?? new List<User>();
        }

        public async Task SetUserReady(string spaceId, string userId, bool isReady)
        {
            await _signalRService.SetUserReady(spaceId, userId, isReady);
        }

        public async Task CheckAllUsersReady(string spaceId)
        {
            await _signalRService.CheckAllUsersReady(spaceId);
        }

        public async Task AddMessage(string spaceId, Message message)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync($"api/space/{spaceId}/messages", message);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Message>> GetMessages(string spaceId)
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<List<Message>>($"api/space/{spaceId}/messages");
        }
    }
}
