using Microsoft.EntityFrameworkCore;
using Skrawl.API.Data.Models;

namespace Skrawl.API.Data
{
    public class SkrawlContext : DbContext
    {
        public SkrawlContext(DbContextOptions<SkrawlContext> options) : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
    }
}