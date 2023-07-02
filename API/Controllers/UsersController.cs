using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // Now we do not need this because we already inherit it from BaseApiController:
    // [ApiController]
    // [Route("api/[controller]")]     // /api/users
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]                   // GET method               // link: /api/users
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()            // if we do not return ActionResult, then we cannot return e.g. Bad Page error (404), etc. (but we still can return users)
        {
            var users = await _context.Users.ToListAsync();

            return users;
        }

        [HttpGet("{id}")]                                       //  link: /api/users/3 (example)
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            return await _context.Users.FindAsync(id);
        }


    }
}