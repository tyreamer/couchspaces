using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using couchspacesShared.Models;

namespace couchspacesShared.Services
{
    public class UserService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public UserService(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<User> GetUserAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? user.FindFirst(ClaimTypes.Email)?.Value
                             ?? string.Empty;

                return new User
                {
                    Id = userId,
                    Name = user.FindFirst(ClaimTypes.Name)?.Value ?? "User",
                    Claims = user.Claims.ToDictionary(c => c.Type, c => c.Value)
                };
            }

            return new User
            {
                Id = string.Empty,
                Name = "User",
                Claims = new Dictionary<string, string>()
            };
        }
    }
}
