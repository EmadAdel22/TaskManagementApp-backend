using Jose.native;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.Data;
using TaskManagement.Models;

using TaskManagement.helpers;

namespace TaskManagement.Controllers
{
   
        [Route("api/[controller]")]
        [ApiController]
        public class AuthController : ControllerBase
        {
            private readonly AppDbContext _context;
            private readonly jwtSettings _jwtSettings;

            public AuthController(AppDbContext context, IOptions<jwtSettings> jwtSettings)
            {
                _context = context;
                _jwtSettings = jwtSettings.Value;
            }

            [HttpPost("register")]
            public async Task<IActionResult> Register(user user)
            {
                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                    return BadRequest("Email already exists");

                // Hash password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }

            [HttpPost("login")]
            public async Task<IActionResult> Login(user login)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(login.PasswordHash, user.PasswordHash))
                    return Unauthorized("Invalid credentials");

                // Create JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name)
                }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    Issuer = _jwtSettings.Issuer,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new { token = tokenHandler.WriteToken(token) });
            }
        }
}

