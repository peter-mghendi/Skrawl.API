using Microsoft.EntityFrameworkCore;

namespace Skrawl.API.Models
{
    public class NoteContext : DbContext
    {
        public NoteContext(DbContextOptions<NoteContext> options) : base(options)
        {
        }

        public DbSet<Note> NoteItems { get; set; }
    }
}