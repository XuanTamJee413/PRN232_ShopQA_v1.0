using DataAccess.Context;
using DomainUser = Domain.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQAPresentation.ViewModels;

namespace ShopQAPresentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ShopQADbContext _context;

        public AuthController(ShopQADbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    (u.Username == model.UsernameOrEmail || u.Email == model.UsernameOrEmail));

            if (user == null)
                return Unauthorized(new { message = "Tài khoản không tồn tại" });
            if (user.PasswordHash != model.Password)
                return Unauthorized(new { message = "Mật khẩu không đúng" });
            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Password != model.ConfirmPassword)
                return BadRequest(new { message = "Mật khẩu không khớp" });

            var isExist = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (isExist)
                return Conflict(new { message = "Email đã tồn tại" });

            var user = new DomainUser
            {
                Username = model.Email,
                Email = model.Email,
                PasswordHash = model.Password, 
                Role = "Customer"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đăng ký thành công" });
        }


    }
}
