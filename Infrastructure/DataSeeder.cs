using System.Collections.Generic;
using System.Linq;
using Domain.Entities;

namespace Infrastructure
{
    public static class DataSeeder
    {
        public static void Initialize(WebApiContext context)
        {
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new (){ Fullname = "Luiz Paulo", Email = "luiz.barroca@edge.ufal.br", Password = "Aa123456!", IsActive = true},
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }
    }
}