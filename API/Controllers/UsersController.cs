using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
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
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;

        }

        // [AllowAnonymous]
        [HttpGet]                   // GET method               // link: /api/users
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()            // if we do not return ActionResult, then we cannot return e.g. Bad Page error (404), etc. (but we still can return users)
        {
            var users = await _userRepository.GetMembersAsync();

            return Ok(users);               // return 200 OK reponse
        }

        [HttpGet("{username}")]                                       //  link: /api/users/3 (example)
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {            
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());      // from this point on, we track what's happening to the var "user"
            
            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);         // do update the user

            // save changes to the DB
            if (await _userRepository.SaveAllAsync()) return NoContent();       // return 204 No Content (it is just an update)
                                                                                // because we have nothing to return, it's just to update data in the DB

            return BadRequest("Failed to update user");             // e.g., if NO changes were actually made (that is, updateDto props are the same as the ones stored in the DB)
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
                // return 201 Created Reponse (along with location details about where to find the newly created resource; but we also send back the newly created resource as well)
                // so basically this will return the PhotoDto object ...
                // ... AS WELL AS "Location" Header containing link similar to "https://localhost:5001/api/Users/lisa" (a combination of first two params of the return CreatedAtAction())
                
                // now this follows REST principles
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("this is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Problem setting the main photo");
        }

    }
}