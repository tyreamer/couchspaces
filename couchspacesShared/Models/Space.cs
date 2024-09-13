using System.Reflection;
using System.Text.Json.Serialization;

namespace couchspacesShared.Models
{
    public class Space
    {
        public required string Id { get; set; }
        public string? Name { get; set; }
        public List<User> Users { get; set; } = [];
        public string? ContentTitle { get; set; }
        public string? Platform { get; set; }
        public string? StreamingPlatform { get; set; }
        public string? StreamingLink { get; set; }
        public ContentType? ContentType { get; set; }
        public bool IsReady { get; set; }
    }

    public class ContentType
    {
        public string Name { get; private set; }

        [JsonConstructor]
        public ContentType(string name)
        {
            Name = name;
        }

        public static ContentType LiveSports { get; } = new ContentType("Live Sports");
        public static ContentType LiveEvents { get; } = new ContentType("Live Events");
        public static ContentType TVShows { get; } = new ContentType("TV Shows");
        public static ContentType Movies { get; } = new ContentType("Movies");
        public static ContentType Other { get; } = new ContentType("Other");

        public static IEnumerable<ContentType> ListAll()
        {
            return typeof(ContentType)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(ContentType))
                .Select(p => (ContentType)p.GetValue(null)!);
        }
    }
}
