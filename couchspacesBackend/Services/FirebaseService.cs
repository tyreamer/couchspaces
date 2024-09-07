using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;

namespace couchspacesBackend.Services
{
    public class FirebaseService
    {
        public FirebaseService(IConfiguration configuration)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(configuration["Firebase:ServiceAccountKeyPath"])
                });
            }
        }
    }
}
