using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly WebApiContext _context;

        public UserRepository(WebApiContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<User> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var storedUser = await FindAsync(cancellationToken, user.Id);
            if (storedUser == null) return await Task.FromResult(user);
            
            _context.Entry(storedUser).CurrentValues.SetValues(user);
            _context.Entry(storedUser).State = EntityState.Modified;
            _context.Entry(storedUser).Property(u => u.Password).IsModified = false;
            await _context.SaveChangesAsync(cancellationToken);

            return storedUser;
        }
    }
}