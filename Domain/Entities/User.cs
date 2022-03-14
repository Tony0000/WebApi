using System.Text.Json.Serialization;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public bool IsActive { get; set; }
        [JsonIgnore]
        private string _password;
        [JsonIgnore]
        public string Password
        {
            get => _password;
            set    
            {
                if (string.IsNullOrEmpty(value)) throw new PasswordNullException("New Password cannot be null.");
                var passwordHasher = new PasswordHasher<User>();
                _password = passwordHasher.HashPassword(this, value);
            }
        }

        public User()
        {
            // inicialiazar todas as listas declaradas aqui para evitar null exceptions ao acessa-las
            // e.g.: subordinados = new List<User>();
        }
    }
}