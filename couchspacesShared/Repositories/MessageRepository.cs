using couchspacesShared.Models;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace couchspacesShared.Repositories
{
    public class MessageRepository
    {
        private readonly IDatabase _redisDatabase;

        public MessageRepository(IConnectionMultiplexer redis)
        {
            _redisDatabase = redis.GetDatabase();
        }

        public async Task AddMessageAsync(string spaceId, Message message)
        {
            var messageJson = JsonSerializer.Serialize(message);
            await _redisDatabase.ListRightPushAsync($"spaceMessages:{spaceId}", messageJson);
        }

        public async Task<List<Message>> GetMessagesAsync(string spaceId)
        {
            var messageJsons = await _redisDatabase.ListRangeAsync($"spaceMessages:{spaceId}");
            var messages = new List<Message>();

            foreach (var messageJson in messageJsons)
            {
                var message = JsonSerializer.Deserialize<Message>(messageJson);
                if (message != null)
                {
                    messages.Add(message);
                }
            }

            return messages;
        }
    }
}
