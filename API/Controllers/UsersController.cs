using System.Security.Claims;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Now we do not need this because we already inherit it from BaseApiController:
    // [ApiController]
    // [Route("api/[controller]")]     // /api/users

    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;

        }

        // [AllowAnonymous]
        [HttpGet]                   // GET method               // link: /api/users
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()            // if we do not return ActionResult, then we cannot return e.g. Bad Page error (404), etc. (but we still can return users)
        {
            var users = await _userRepository.GetMembersAsync();

            return Ok(users);
        }

        [HttpGet("{username}")]                                       //  link: /api/users/3 (example)
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // Keep in mind the "User" below comes not from your code, but from the System (ControllerBase) ! ! !
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;        // similar to NameId in the TokenService
            // don't forget that we already automatically in the background recieve a token back from the API, so from this recieved token we can actually understand if it is the actual user who he/she claims to be
            
            var user = await _userRepository.GetUserByUsernameAsync(username);      // from this point on, we track what's happening to the var "user"
            
            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);         // do update the user

            // save changes to the DB
            if (await _userRepository.SaveAllAsync()) return NoContent();      // because we have nothing to return, it's just to update data in the DB

            return BadRequest("Failed to update user");             // e.g., if NO changes were actually made (that is, updateDto props are the same as the ones stored in the DB)
        }

    }
}