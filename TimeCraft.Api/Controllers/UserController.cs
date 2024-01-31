using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeCraft.Core.Services.UserService;
using TimeCraft.Domain.Dtos.UserDtos;

namespace TimeCraft.Api.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("User")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userService.GetUser(id);
            return Ok(user);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        [HttpGet("UserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst("Id").Value;
            var user = await _userService.GetUser(userId);
            if (user is null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers(int page = 1, int pageSize = 10)
        {
            var users = await _userService.GetUsers(page, pageSize);
            return Ok(users);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        [HttpGet("UserRole")]
        public async Task<IActionResult> GetUserRole()
        {
            var userData = (ClaimsIdentity)User.Identity;
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role is null)
            {
                return BadRequest("The user doesn't have any roel!");
            }
            return Ok(role);
        }

        [HttpPost("User")]
        public async Task<IActionResult> CreateUser(CreateUserDto userToCreate)
        {
            await _userService.CreateUser(userToCreate);
            return Ok(userToCreate);
        }

        [HttpPut("User")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto userToUpdate)
        {
            await _userService.UpdateUser(userToUpdate);
            return Ok(userToUpdate);
        }

        [HttpDelete("User")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _userService.DeleteUser(id);
            return Ok();
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistration user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUser(user);
                if (result.Succedded)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Login(user);
                if (result.Succedded)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest();
        }
    }
}
