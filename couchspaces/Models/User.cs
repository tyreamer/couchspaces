namespace couchspaces.Models
{
    public class User
    {
        public static bool IsAuthenticated { get; set; }
        public Dictionary<string, string> Claims { get; set; } = [];
        public required string Id { get; set; }
        public required string Name { get; set; }
        public bool IsReady { get; set; }
    }
}
