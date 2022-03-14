using System;

namespace Domain.Models.Results
{
    public class AuthenticationResult
    {
        public bool Authenticated { get; set; }
        public DateTime ExpiresAt { get; set; }
        public LoginResult User { get; set; }
        public string Error { get; set; }
    }
}