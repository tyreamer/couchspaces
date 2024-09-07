using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace couchspaces.Services
{
    public class TokenValidationService(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<bool> ValidateTokenAsync(string idToken)
        {
            var response = await _httpClient.PostAsJsonAsync("api/firebase/validate-token", idToken);
            return response.IsSuccessStatusCode;
        }
    }
}
