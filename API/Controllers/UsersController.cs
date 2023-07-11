using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // Now we do not need this because we already inherit it from BaseApiController:
    // [ApiController]
    // [Route("api/[controller]")]     // /api/users

    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;

        }

        // [AllowAnonymous]
        [HttpGet]                   // GET method               // link: /api/users
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()            // if we do not return ActionResult, then we cannot return e.g. Bad Page error (404), etc. (but we still can return users)
        {
            return Ok(await _userRepository.GetUsersAsync());
        }

        [HttpGet("{username}")]                                       //  link: /api/users/3 (example)
        public async Task<ActionResult<AppUser>> GetUser(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }


    }
}