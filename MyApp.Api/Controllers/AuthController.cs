using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BidFlow.Api.Data;
using BidFlow.Api.DTOs;
using BidFlow.Entities;

namespace BidFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new
                {
                    success = false,
                    message = "Email already exists",
                    error = "EMAIL_ALREADY_EXISTS"
                });

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest(new
                {
                    success = false,
                    message = "Username already exists",
                    error = "USERNAME_ALREADY_EXISTS"
                });

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "User created successfully",
                data = new
                {
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        email = user.Email,
                        isPro = user.IsPro,
                        isAdmin = user.IsAdmin
                    }
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid credentials",
                    error = "INVALID_CREDENTIALS"
                });

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                success = true,
                message = "Login successful",
                data = new
                {
                    token = token,
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        email = user.Email,
                        role = user.IsAdmin ? "Admin" : (user.IsPro ? "Pro" : "User"),
                        isPro = user.IsPro,
                        isAdmin = user.IsAdmin
                    }
                }
            });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("IsAdmin", user.IsAdmin.ToString()),
                new Claim("IsPro", user.IsPro.ToString()),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : (user.IsPro ? "Pro" : "User"))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}