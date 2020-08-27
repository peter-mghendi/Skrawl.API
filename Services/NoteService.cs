using Skrawl.API.Data.Models;

namespace Skrawl.API.Services
{
    class NoteService : INoteService
    {
        public NoteDTO ItemToDTO(Note note)
        {
            return new NoteDTO
            {
                Id = note.Id,
                Title = note.Title,
                Body = note.Body
            };
        }
    }
}