using Microsoft.JSInterop;

namespace couchspaces.Services
{
    public class FirebaseService(IJSRuntime jsRuntime)
    {
        private readonly IJSRuntime _jsRuntime = jsRuntime;

        public async Task<Dictionary<string, string>> SignInWithGoogleAsync()
        {
            return await _jsRuntime.InvokeAsync<Dictionary<string, string>>("firebaseAuth.signInWithGoogle");
        }
    }
}