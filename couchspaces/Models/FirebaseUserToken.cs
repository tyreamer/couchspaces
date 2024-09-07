using Newtonsoft.Json;

namespace couchspaces.Models
{
    public class FirebaseUserToken
    {
        [JsonProperty("Access_Token")]
        public string AccessToken { get; set; }
        [JsonProperty("Expires_In")]
        public string ExpiresIn { get; set; }
        [JsonProperty("Token_Type")]
        public string TokenType { get; set; }
        [JsonProperty("Refresh_Token")]
        public string RefreshToken { get; set; }
        [JsonProperty("Id_Token")]
        public string IdToken { get; set; }
    }
}
