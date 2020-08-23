using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skrawl.API.Data;
using Skrawl.API.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Skrawl.API.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly SkrawlContext _context;
        private readonly IPasswordService _passwordService;

        public SkrawlContext Context { get => _context; }

        // inject your database here for user validation
        public UserService(ILogger<UserService> logger, SkrawlContext context, IPasswordService passwordService)
        {
            _logger = logger;
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<bool> IsValidUserCredentialsAsync(string email, string password)
        {
            _logger.LogInformation($"Validating user [{email}]");

            if (string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(password)
                || !await IsAnExistingUserAsync(email))
            {
                return false;
            }

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            User user = await _context.Users.SingleAsync(u => u.Email == email);

            return _passwordService.VerifyHash(passwordBytes, user.Salt, user.Password);
        }

        public async Task<bool> IsAnExistingUserAsync(long id) => 
            await _context.Users.AnyAsync(e => e.Id == id);

        public async Task<bool> IsAnExistingUserAsync(string email) => 
            await _context.Users.AnyAsync(e => e.Email.Equals(email));

        public async Task<string> GetUserRoleAsync(string email)
        {
            if (!await IsAnExistingUserAsync(email))
            {
                return string.Empty;
            }

            User user = await _context.Users.SingleAsync(x => x.Email.Equals(email));

            if (user == null) _logger.LogInformation("Null user");

            return user.Role;
        }
    }
}