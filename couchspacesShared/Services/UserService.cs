// File: couchspacesShared/Services/UserService.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace couchspacesShared.Services
{
    public class UserService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public UserService(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<string> GetUsernameAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                return user.FindFirst(ClaimTypes.Name)?.Value ?? "User";
            }

            return "User";
        }
    }
}
