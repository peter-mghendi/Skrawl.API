using System.Threading.Tasks;
using Skrawl.API.Data;

namespace Skrawl.API.Services
{
    public interface IUserService
    {
        public SkrawlContext Context { get; }

        public Task<bool> IsAnExistingUserAsync(long id);
        public Task<bool> IsAnExistingUserAsync(string email);
        public Task<bool> IsValidUserCredentialsAsync(string email, string password);
        public Task<string> GetUserRoleAsync(string email);
    }
}