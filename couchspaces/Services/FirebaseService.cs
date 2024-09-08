using Microsoft.JSInterop;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using couchspaces.Models;
using System.Text.Json;

namespace couchspaces.Services
{
    public class FirebaseService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public FirebaseService(IJSRuntime jsRuntime, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _jsRuntime = jsRuntime;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<FirebaseUser> SignInWithGoogleAsync()
        {
            var user = await _jsRuntime.InvokeAsync<Dictionary<string, object>>("firebaseAuth.signInWithGoogle");

            if (user == null)
            {
                return null;
            }

            if (!user.TryGetValue("stsTokenManager", out var stsTokenManagerJson) || stsTokenManagerJson == null)
            {
                throw new KeyNotFoundException("The key 'stsTokenManager' was not found in the dictionary.");
            }

            // Deserialize the stsTokenManager using JsonElement
            var stsTokenManagerElement = (JsonElement)stsTokenManagerJson;
            var stsTokenManager = new StsTokenManager
            {
                RefreshToken = stsTokenManagerElement.GetProperty("refreshToken").GetString(),
                AccessToken = stsTokenManagerElement.GetProperty("accessToken").GetString(),
                ExpirationTime = stsTokenManagerElement.GetProperty("expirationTime").GetString()
            };

            var firebaseUser = new FirebaseUser
            {
                Uid = user["uid"]?.ToString(),
                DisplayName = user["displayName"]?.ToString(),
                PhotoURL = user["photoURL"]?.ToString(),
                Email = user["email"]?.ToString(),
                EmailVerified = bool.Parse(user["emailVerified"]?.ToString() ?? "false"),
                PhoneNumber = user["phoneNumber"]?.ToString(),
                IsAnonymous = bool.Parse(user["isAnonymous"]?.ToString() ?? "false"),
                TenantId = user["tenantId"]?.ToString(),
                StsTokenManager = stsTokenManager,
                LastLoginAt = user["lastLoginAt"]?.ToString(),
                CreatedAt = user["createdAt"]?.ToString()
            };

            // Log the values of the required fields
            Console.WriteLine($"Email: {firebaseUser.Email}");
            Console.WriteLine($"StsTokenManager: {firebaseUser.StsTokenManager}");
            Console.WriteLine($"AccessToken: {firebaseUser.StsTokenManager?.AccessToken}");

            // Ensure required fields are not null
            if (string.IsNullOrEmpty(firebaseUser.Email) || firebaseUser.StsTokenManager == null || string.IsNullOrEmpty(firebaseUser.StsTokenManager.AccessToken))
            {
                throw new InvalidOperationException("User data is missing required fields.");
            }

            // Save the user to local storage
            await _localStorage.SetItemAsync("couchspacesUser", firebaseUser);

            var customAuthStateProvider = (CouchspacesAuthenticationStateProvider)_authStateProvider;
            await customAuthStateProvider.MarkUserAsAuthenticated(firebaseUser);

            return firebaseUser;
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
