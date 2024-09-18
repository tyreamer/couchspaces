using couchspacesShared.Models;
using couchspacesShared.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace couchspacesBackend.Services
{
    public class MessageService
    {
        private readonly MessageRepository _messageRepository;

        public MessageService(MessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task AddMessageAsync(string spaceId, Message message)
        {
            await _messageRepository.AddMessageAsync(spaceId, message);
        }

        public async Task<List<Message>> GetMessagesAsync(string spaceId)
        {
            return await _messageRepository.GetMessagesAsync(spaceId);
        }
    }
}
