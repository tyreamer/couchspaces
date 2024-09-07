using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace couchspaces.Services
{
    public class CouchspacesAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IJSRuntime _jsRuntime;

        public CouchspacesAuthenticationStateProvider(ILocalStorageService localStorage, IJSRuntime jsRuntime)
        {
            _localStorage = localStorage;
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = await _localStorage.GetItemAsync<Dictionary<string, string>>("couchspacesUser");
            if (user == null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user["displayName"]),
                new Claim(ClaimTypes.Email, user["email"]),
                new Claim("Token", user["stsTokenManager"])
            }, "firebaseAuth");

            var claimsPrincipal = new ClaimsPrincipal(identity);
            return new AuthenticationState(claimsPrincipal);
        }

        public async Task MarkUserAsAuthenticated(Dictionary<string, string> user)
        {
            await _localStorage.SetItemAsync("couchspacesUser", user);
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user["displayName"]),
                new Claim(ClaimTypes.Email, user["email"]),
                new Claim("Token", user["stsTokenManager"])
            }, "firebaseAuth");

            var claimsPrincipal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.RemoveItemAsync("couchspacesUser");
            var identity = new ClaimsIdentity();
            var claimsPrincipal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}
