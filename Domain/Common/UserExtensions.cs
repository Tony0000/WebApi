using System.Text.RegularExpressions;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain.Common
{
    public static class UserExtensions
    {
        private static readonly Regex HasNumber = new Regex(@"[0-9]+");
        private static readonly Regex HasUpperChar = new Regex(@"[A-Z]+");
        private static readonly Regex HasLowerChar = new Regex(@"[a-z]+");
        private static readonly Regex HasSpecialChar = new Regex(@"[!@#$%^&+=\-*\[\](){}]+");

        public static bool ValidateRuleSet(this User user, string password)
        {
            return user.Email != password
                   && user.Fullname != password
                   && HasNumber.IsMatch(password)
                   && HasUpperChar.IsMatch(password)
                   && HasLowerChar.IsMatch(password)
                   && HasSpecialChar.IsMatch(password);
        }
        
        
        public static bool ConfirmPassword(this User user, string password)
        {
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher
                .VerifyHashedPassword(user, user.Password, password);
            return result != PasswordVerificationResult.Failed;
        }
    }
}