using System.Security.Claims;
using Blazored.LocalStorage;
using couchspaces.Models;
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
            try
            {
                var user = await _localStorage.GetItemAsync<FirebaseUser>("couchspacesUser");
                if (user == null)
                {
                    Console.WriteLine("No user found in local storage.");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                if (string.IsNullOrEmpty(user.DisplayName) || string.IsNullOrEmpty(user.Email) || user.StsTokenManager == null)
                {
                    Console.WriteLine("User data is missing required fields.");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                Console.WriteLine($"GetAuthenticationStateAsync User Found: {user.DisplayName}");

                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.DisplayName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("Token", user.StsTokenManager.AccessToken)
                }, "firebaseAuth");

                var claimsPrincipal = new ClaimsPrincipal(identity);
                return new AuthenticationState(claimsPrincipal);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving authentication state: {ex.Message}");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task MarkUserAsAuthenticated(FirebaseUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentNullException(nameof(user.Email));
            }

            // Store the user in local storage
            await _localStorage.SetItemAsync("couchspacesUser", user);

            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.DisplayName),
                new (ClaimTypes.Email, user.Email),
                new ("Token", user.StsTokenManager.AccessToken)
            };

            var identity = new ClaimsIdentity(claims, "firebaseAuth");
            var userPrincipal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(userPrincipal)));
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
