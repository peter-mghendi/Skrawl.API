using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skrawl.API.Data;
using Skrawl.API.Data.Models;
using Skrawl.API.Services;

namespace Skrawl.API.Controllers
{
    [Route("api/me/notes")]
    [ApiController]
    [Authorize]
    public class UserNotesController : ControllerBase
    {
        private readonly ILogger<UserNotesController> _logger;
        private readonly SkrawlContext _context;
        private readonly IUserService _userService;
        private readonly INoteService _noteService;

        public UserNotesController(ILogger<UserNotesController> logger, SkrawlContext context, IUserService userService, INoteService noteService)
        {
            _logger = logger;
            _context = context;
            _userService = userService;
            _noteService = noteService;
        }

        // GET: api/me/notes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoteDTO>>> GetNotes()
        {
            var user = await _userService.FindUserByEmailAsync(User.Identity.Name);
            
            if (user == null)
            {
                return BadRequest();
            }

            return await _context.Notes.Where(n => n.UserId == user.Id)
                .Select(n => _noteService.ItemToDTO(n)).ToListAsync();
        }

        // GET: api/me/notes/id
        [HttpGet("{id}")]
        public async Task<ActionResult<NoteDTO>> GetNote(long id)
        {
            var user = await _userService.FindUserByEmailAsync(User.Identity.Name);
            
            if (user == null)
            {
                return BadRequest();
            }

            var note = await _context.Notes
                .SingleOrDefaultAsync(n => n.Id == id && n.UserId == user.Id);

            if (note == null)
            {
                return NotFound();
            }

            return _noteService.ItemToDTO(note);
        }

        // PUT: api/me/notes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(long id, NoteDTO noteDTO)
        {
            if (id != noteDTO.Id)
            {
                return BadRequest();
            }

            var user = await _userService.FindUserByEmailAsync(User.Identity.Name);
            
            if (user == null)
            {
                return BadRequest();
            }

            var note = await _context.Notes.FindAsync(id);

            if (note == null)
            {
                return BadRequest();
            }

            if (note.UserId != user.Id)
            {
                return Forbid();
            }
           
           note.Title = noteDTO.Title;
           note.Body = noteDTO.Body;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoteExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // POST: api/me/notes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<NoteDTO>> PostNote(NoteDTO noteDTO)
        {
            var user = await _userService.FindUserByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return BadRequest();
            }

            var note = new Note{
                Title = noteDTO.Title,
                Body = noteDTO.Body,
                UserId = user.Id
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, _noteService.ItemToDTO(note));
        }

        // DELETE: api/me/notes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<NoteDTO>> DeleteNote(long id)
        {
            var user = await _userService.FindUserByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return BadRequest();
            }

            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            if (note.UserId != user.Id)
            {
                return Forbid();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return _noteService.ItemToDTO(note);
        }

        private bool NoteExists(long id)
        {
            return _context.Notes.Any(e => e.Id == id);
        }
    }
}
