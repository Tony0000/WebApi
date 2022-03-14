using System.Security.Claims;
using Domain.Common;

namespace WebApi.Common
{
    /// <summary>
    /// Claims Principal related extensions for <see cref="ClaimsPrincipal"/>.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        public static int GetLoggedUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirstValue(AuthClaimNames.UserId));
        }
        
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return bool.Parse(user.FindFirstValue(AuthClaimNames.IsAdmin));
        }
    }
}