using BidFlow.Api.Data;
using BidFlow.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BidFlow.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/users")]
    public class AdminUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            user.Id = Guid.NewGuid();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            _context.Users.Add(user);
            await LogAction("Create", user.Id);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, User updated)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Email = updated.Email;
            user.Username = updated.Username;
            user.IsAdmin = updated.IsAdmin;
            user.IsPro = updated.IsPro;
            
            if (!string.IsNullOrWhiteSpace(updated.PasswordHash))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updated.PasswordHash);

            await LogAction("Update", id);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await LogAction("Delete", id);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetUserActivityLogs()
        {
            var logs = await _context.UserActivityLogs
                .Include(l => l.PerformedByUser)
                .Include(l => l.TargetUser)
                .OrderByDescending(l => l.Timestamp)
                .Select(l => new
                {
                    LogId = l.Id,
                    Action = l.Action,
                    Timestamp = l.Timestamp,
                    PerformedBy = new
                    {
                        l.PerformedByUser.Id,
                        l.PerformedByUser.Username,
                        l.PerformedByUser.Email
                    },
                    TargetUser = new
                    {
                        l.TargetUser.Id,
                        l.TargetUser.Username,
                        l.TargetUser.Email
                    }
                })
                .ToListAsync();

            return Ok(logs);
        }


        private async Task LogAction(string action, Guid targetUserId)
        {
            var performedById = Guid.Parse(User.Claims.First(c => c.Type == "UserId").Value);
            _context.UserActivityLogs.Add(new UserActivityLog
            {
                Action = action,
                PerformedByUserId = performedById,
                TargetUserId = targetUserId
            });
        }
    }
}
