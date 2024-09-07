using Microsoft.JSInterop;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace couchspaces.Services
{
    public class FirebaseService(IJSRuntime jsRuntime, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
    {
        private readonly IJSRuntime _jsRuntime = jsRuntime;
        private readonly ILocalStorageService _localStorage = localStorage;
        private readonly AuthenticationStateProvider _authStateProvider = authStateProvider;

        public async Task<Dictionary<string, string>> SignInWithGoogleAsync()
        {
            var user = await _jsRuntime.InvokeAsync<Dictionary<string, object>>("firebaseAuth.signInWithGoogle");

            if (user != null)
            {
                // Convert all values to strings
                var stringifiedUser = user.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString());

                var customAuthStateProvider = (CouchspacesAuthenticationStateProvider)_authStateProvider;
                await customAuthStateProvider.MarkUserAsAuthenticated(stringifiedUser);

                return stringifiedUser;
            }

            return null;
        }

        public async Task SignOutAsync()
        {
            await _jsRuntime.InvokeVoidAsync("firebaseAuth.signOut");
            var customAuthStateProvider = (CouchspacesAuthenticationStateProvider)_authStateProvider;
            await customAuthStateProvider.MarkUserAsLoggedOut();
        }

        public async Task<Dictionary<string, string>> GetUserFromLocalStorageAsync()
        {
            return await _localStorage.GetItemAsync<Dictionary<string, string>>("couchspacesUser");
        }
    }
}
