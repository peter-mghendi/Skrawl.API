using System.Threading.Tasks;
using Skrawl.API.Data;
using Skrawl.API.Data.Models;

namespace Skrawl.API.Services
{
    public interface IUserService
    {
        public SkrawlContext Context { get; }

        public Task<bool> IsAnExistingUserAsync(long id);
        public Task<bool> IsAnExistingUserAsync(string email);
        public Task<bool> IsValidUserCredentialsAsync(string email, string password);
        public Task<string> GetUserRoleAsync(string email);
        public UserDTO ItemToDTO(User user);
        public Task<User> CreateUser(User user);
    }
}