using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OcelotGateway.DTO;
using OcelotGateway.Model;

namespace OcelotGateway.Controllers
{
    [ApiController]
    [Route("gateway/auth")]
    public class AuthController : ControllerBase
    {
        private readonly GatewayDbContext _db;
        private readonly JwtService _jwt;

        public AuthController(GatewayDbContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (existing != null)
                return BadRequest("Username already taken.");

            var user = new AppUser
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok(new { user.Id, user.Username });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var token = _jwt.GenerateToken(user);
            return Ok(new { token });
        }
    }
}
