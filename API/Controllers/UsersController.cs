using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]     // /api/users
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]                   // GET method               // link: /api/users
        public ActionResult<IEnumerable<AppUser>> GetUsers()            // if we do not return ActionResult, then we cannot return e.g. Bad Page error (404), etc. (but we still can return users)
        {
            var users = _context.Users.ToList();

            return users;
        }

        [HttpGet("{id}")]                                       //  link: /api/users/3 (example)
        public ActionResult<AppUser> GetUser(int id)
        {
            return _context.Users.Find(id);
        }


    }
}