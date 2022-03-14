using System.Security;

namespace WebApi.Exceptions
{
    public class ForbiddenException : SecurityException
    {
        public ForbiddenException(string message) : base(message)
        {
            
        }
    }
}