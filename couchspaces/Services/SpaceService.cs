using couchspaces.Models;

namespace couchspaces.Services
{
    public class SpaceService
    {
        private readonly List<Space> _spaces = [];

        public Space CreateSpace(string name)
        {
            var space = new Space
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Users = [] // Initialize the Users list
            };

            _spaces.Add(space);
            return space;
        }

        public Space GetSpace(string id)
        {
            return _spaces.FirstOrDefault(s => s.Id == id) ?? throw new Exception("Unable to get space.");
        }

        public void AddUserToSpace(string spaceId, User user)
        {
            var space = GetSpace(spaceId);
            space?.Users.Add(user);
        }

        public void SetUserReady(string spaceId, string userId, bool isReady)
        {
            var space = GetSpace(spaceId);
            var user = space?.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.IsReady = isReady;
            }
        }

        public bool AllUsersReady(string spaceId)
        {
            var space = GetSpace(spaceId);
            return space?.Users.All(u => u.IsReady) ?? false;
        }

        // New method to get users in a space
        public List<User> GetUsersInSpace(string spaceId)
        {
            var space = GetSpace(spaceId);
            return space?.Users ?? [];
        }
    }
}
