using DataAccess.Context;
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
                    (u.Username == model.UsernameOrEmail || u.Email == model.UsernameOrEmail)
                    && u.PasswordHash == model.Password 
                );

            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role
            });
        }
    }
}
