using couchspacesShared.Models;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace couchspacesBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpaceController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, Space> _spaces = new();
        private static readonly ConcurrentDictionary<string, List<couchspacesShared.Models.Message>> _messages = new();

        [HttpPost]
        public IActionResult CreateSpace([FromBody] Space space)
        {
            space.Id = Guid.NewGuid().ToString();
            _spaces[space.Id] = space;
            return Ok(space);
        }

        [HttpGet("{id}")]
        public IActionResult GetSpace(string id)
        {
            if (_spaces.TryGetValue(id, out var space))
            {
                return Ok(space);
            }
            return NotFound();
        }

        [HttpGet("exists/{id}")]
        public IActionResult SpaceExists(string id)
        {
            return Ok(_spaces.ContainsKey(id));
        }

        [HttpPost("{spaceId}/users")]
        public IActionResult AddUserToSpace(string spaceId, [FromBody] User user)
        {
            if (_spaces.TryGetValue(spaceId, out var space))
            {
                space.Users.Add(user);
                return Ok();
            }
            return NotFound();
        }

        [HttpPost("{spaceId}/messages")]
        public IActionResult AddMessage(string spaceId, [FromBody] couchspacesShared.Models.Message message)
        {
            if (!_messages.ContainsKey(spaceId))
            {
                _messages[spaceId] = new List<couchspacesShared.Models.Message>();
            }
            _messages[spaceId].Add(message);
            return Ok();
        }

        [HttpGet("{spaceId}/messages")]
        public IActionResult GetMessages(string spaceId)
        {
            if (_messages.TryGetValue(spaceId, out var messages))
            {
                return Ok(messages);
            }
            return NotFound();
        }
    }
}
