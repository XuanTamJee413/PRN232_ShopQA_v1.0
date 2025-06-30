using Business.DTO;
using Business.Iservices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShopQAPresentation.Controllers.Profile
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        // Xem profile
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var user = await _userService.GetProfileAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // Đổi mật khẩu
        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            // Kiểm tra mật khẩu cũ
            var isValid = await _userService.ValidatePasswordAsync(id, request.OldPassword);
            if (!isValid)
                return BadRequest("Mật khẩu cũ không đúng.");
            await _userService.ChangePasswordAsync(id, request.NewPassword);
            return NoContent();
        }

        // Cập nhật thông tin cá nhân
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePersonalInfo(int id, [FromBody] UserDTO userDto)
        {
            if (id != userDto.Id) return BadRequest();
            await _userService.UpdatePersonalInfoAsync(userDto);
            return NoContent();
        }
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
