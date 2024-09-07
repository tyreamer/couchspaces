using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace couchspaces.Services
{
    public class StateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage = localStorage;

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            try
            {
                var userId = await _localStorage.GetItemAsync<string>("userId");
                var userEmail = await _localStorage.GetItemAsync<string>("userEmail");
                var userName = await _localStorage.GetItemAsync<string>("userName");

                if (!string.IsNullOrEmpty(userId))
                {
                    var claims = new List<Claim>
                    {
                        new (ClaimTypes.NameIdentifier, userId),
                        new (ClaimTypes.Email, userEmail ?? string.Empty),
                        new (ClaimTypes.Name, userName ?? string.Empty)
                    };
                    identity = new ClaimsIdentity(claims, "authentication");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Request failed:" + ex.ToString());
            }
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyUserAuthentication(ClaimsPrincipal user)
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            var identity = new ClaimsIdentity();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
        }
    }
}
