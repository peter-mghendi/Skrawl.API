using System.Text;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skrawl.API.Services;
using Skrawl.API.Infrastructure;
using Skrawl.API.Data.Models;
using Microsoft.Extensions.Options;

namespace Skrawl.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IPasswordService _passwordService;

        public UsersController(
            ILogger<UsersController> logger,
            IUserService userService,
            IJwtAuthManager jwtAuthManager,
            IPasswordService passwordService)
        {
            _logger = logger;
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
            _passwordService = passwordService;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(long id)
        {
            var user = await _userService.Context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return _userService.ItemToDTO(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _userService.Context.Entry(user).State = EntityState.Modified;

            try
            {
                await _userService.Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _userService.IsAnExistingUserAsync(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostUser(
            [FromBody] SignupRequest request,
            [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions)
        {
            User user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = Encoding.UTF8.GetBytes(request.Password)
            };

            try
            {
                user = await _userService.CreateUser(user);
            }
            catch (DbUpdateException)
            {
                if (await _userService.IsAnExistingUserAsync(user.Email))
                {
                    ModelState.AddModelError(nameof(user.Email), "That email address is already in use");
                    return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
                }
                else throw;
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, _userService.ItemToDTO(user));
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserDTO>> DeleteUser(long id)
        {
            var user = await _userService.Context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _userService.Context.Users.Remove(user);
            await _userService.Context.SaveChangesAsync();

            return _userService.ItemToDTO(user);
        }
    }
}
