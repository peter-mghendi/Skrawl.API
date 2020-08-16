using Microsoft.EntityFrameworkCore;

namespace Skrawl.API.Models
{
    public class UserContext: DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    } 
}