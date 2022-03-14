using System;
using Domain.Entities;

namespace Domain.Exceptions
{
    public class PasswordNullException : ArgumentNullException
    {
        public PasswordNullException(string message) 
            : base(nameof(User.Password), message)
        {
            
        }
    }
}