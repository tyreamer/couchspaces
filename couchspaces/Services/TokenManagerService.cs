using System.Net.Http.Json;

namespace couchspaces.Services
{
    public class TokenManagerService(HttpClient httpClient, ILogger<TokenManagerService> logger)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<TokenManagerService> _logger = logger;
        private Timer? _timer;
        private string? _idToken;

        public void StartTokenValidation(string idToken)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                _logger.LogError("Token is null or empty. Cannot start token validation.");
                return;
            }

            _idToken = idToken;
            _logger.LogInformation("Starting token validation with token: {Token}", _idToken);
            _timer = new Timer(ValidateToken, null, TimeSpan.Zero, TimeSpan.FromMinutes(5)); // Check every 5 minutes
        }

        private async void ValidateToken(object? state)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/firebase/validate-token", _idToken);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Token is invalid or expired.");
                    // Handle token invalidation (e.g., log out the user, refresh the token, etc.)
                }
                else
                {
                    _logger.LogInformation("Token is valid.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token.");
            }
        }

        public void StopTokenValidation()
        {
            _timer?.Change(Timeout.Infinite, 0);
        }
    }
}
