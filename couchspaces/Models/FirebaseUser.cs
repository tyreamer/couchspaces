namespace couchspaces.Models
{
    public class FirebaseUser
    {
        public string Uid { get; set; }
        public string DisplayName { get; set; } // Changed to string
        public string PhotoURL { get; set; } // Changed to string
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string PhoneNumber { get; set; } // Changed to string
        public bool IsAnonymous { get; set; }
        public string TenantId { get; set; } // Changed to string
        public List<ProviderData> ProviderData { get; set; }
        public StsTokenManager StsTokenManager { get; set; }
        public string RedirectEventId { get; set; } // Changed to string
        public string LastLoginAt { get; set; }
        public string CreatedAt { get; set; }
        public MultiFactor MultiFactor { get; set; }
    }

    public class ProviderData
    {
        public string Uid { get; set; }
        public string DisplayName { get; set; } // Changed to string
        public string PhotoURL { get; set; } // Changed to string
        public string Email { get; set; }
        public string PhoneNumber { get; set; } // Changed to string
        public string ProviderId { get; set; }
    }

    public class StsTokenManager
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public string ExpirationTime { get; set; }
    }

    public class MultiFactor
    {
        public List<object> EnrolledFactors { get; set; }
    }
}
