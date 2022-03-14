using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Common;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository
                .Get()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            
            if (user is null)
                return NotFound<User>();
            
            return Ok(user);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var request = Request.QueryString.ToString().ToDictionary();
            
            var result = await _userRepository
                .Get()
                .AsNoTracking()
                .Search(request)
                .Sort(request)
                .GetPage(request, cancellationToken);

            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Post(User user, CancellationToken cancellationToken)
        {
            if (await _userRepository.Get().AnyAsync(u => u.Email == user.Email, cancellationToken))
                return BadRequest();

            await _userRepository.AddAsync(user, cancellationToken);
            
            return Ok(user);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, User user, CancellationToken cancellationToken)
        {
            if (await _userRepository.Get().AnyAsync(u => u.Id != id && u.Email == user.Email, cancellationToken))
                return BadRequest();

            var modelDb = await _userRepository
                .Get()
                .Where(x => x.Id == id)
                .SingleAsync(cancellationToken);

            modelDb.Email = user.Email;
            modelDb.Username = user.Username;
            modelDb.Fullname = user.Fullname;
            modelDb.IsActive = user.IsActive;
            
            await _userRepository.SaveChangesAsync(cancellationToken);
            
            return await Get(modelDb.Id, CancellationToken.None);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            if (!await _userRepository.Get().AnyAsync(u => u.Id == id, cancellationToken))
                return NotFound<User>();

            await _userRepository.DeleteAsync(u => u.Id == id, cancellationToken);
            
            return NoContent();
        }
    }
}