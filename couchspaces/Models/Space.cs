namespace couchspaces.Models
{
    public class Space
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public List<User> Users { get; set; } = [];
        public string? ContentTitle { get; set; }
        public string? Platform { get; set; }
        public bool IsReady { get; set; }
    }
}
