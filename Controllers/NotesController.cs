using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skrawl.API.Data;
using Skrawl.API.Data.Models;

namespace Skrawl.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly SkrawlContext _context;

        public NotesController(SkrawlContext context)
        {
            _context = context;
        }

        // GET: api/Notes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoteDTO>>> GetNotes() =>
            await _context.Notes.Select(x => ItemToDTO(x)).ToListAsync();

        // GET: api/Notes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NoteDTO>> GetNote(long id)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return ItemToDTO(note);
        }

        // PUT: api/Notes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(long id, NoteDTO noteDTO)
        {
            if (id != noteDTO.Id)
            {
                return BadRequest();
            }

            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            note.Title = noteDTO.Title;
            note.Body = noteDTO.Body;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await NoteExistsAsync(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // POST: api/Notes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<NoteDTO>> PostNote(NoteDTO noteDTO)
        {
            var note = new Note
            {
                Title = noteDTO.Title,
                Body = noteDTO.Body,
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, ItemToDTO(note));
        }

        // DELETE: api/Notes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<NoteDTO>> DeleteNote(long id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return ItemToDTO(note);
        }

        private async Task<bool> NoteExistsAsync(long id)
        {
            return await _context.Notes.AnyAsync(e => e.Id == id);
        }

        private static NoteDTO ItemToDTO(Note note) =>
        new NoteDTO
        {
            Id = note.Id,
            Title = note.Title,
            Body = note.Body
        };
    }
}
