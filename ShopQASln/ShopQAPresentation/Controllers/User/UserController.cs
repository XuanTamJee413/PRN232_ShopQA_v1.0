using Business.DTO;
using Business.Iservices;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShopQAPresentation.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDto)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(userDto);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userDto)
        {
            if (id != userDto.Id)
            {
                return BadRequest(new { message = "Id mismatch between route and body" });
            }

            try
            {
                await _userService.UpdateUserAsync(userDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchUser([FromQuery] string keyword)
        {
            var user = await _userService.FindByEmailOrUsernameAsync(keyword);
            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }


        [HttpGet("filter")]
        public async Task<IActionResult> FilterByRole([FromQuery] string role)
        {
            var users = await _userService.FilterUsersByRoleAsync(role);
            return Ok(users);
        }


        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromQuery] string status)
        {
            await _userService.UpdateAccountStatusAsync(id, status);
            return NoContent();
        }
        [HttpGet("search-users")]
        public async Task<IActionResult> SearchUsers([FromQuery] string? keyword, [FromQuery] string? role)
        {
            var result = await _userService.SearchUsersAsync(keyword, role);
            return Ok(result);
        }

    }
}

