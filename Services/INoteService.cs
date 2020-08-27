using Skrawl.API.Data.Models;

namespace Skrawl.API.Services
{
    public interface INoteService
    {
        public NoteDTO ItemToDTO(Note note);
    }
}