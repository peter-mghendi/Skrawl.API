using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Skrawl.API.Data;
using Skrawl.API.Data.Models;

namespace Skrawl.API.Services
{
    public interface IUserService
    {
        public SkrawlContext Context { get; }

        public DbSet<User> Users { get; }

        public Task<bool> IsAnExistingUserAsync(long id);
        public Task<bool> IsAnExistingUserAsync(string email);
        public Task<bool> IsValidUserCredentialsAsync(string email, string password);
        public Task<string> GetUserRoleAsync(string email);
        public UserDTO ItemToDTO(User user);
        public Task<User> CreateUser(User user);
        public Task<User> FindUserByEmailAsync(string email);
        public Task<User> FindUserByIdAsync(long id);
    }
}