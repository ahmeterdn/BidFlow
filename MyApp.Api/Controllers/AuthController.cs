using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using BidFlow.Services;
using BidFlow.DTOs.Auth;
using BidFlow.Common;

namespace BidFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<RegisterDto> _registerValidator;

        public AuthController(
            IAuthService authService,
            IValidator<LoginDto> loginValidator,
            IValidator<RegisterDto> registerValidator)
        {
            _authService = authService;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var result = Result<LoginDto>.Failure(ErrorMessages.ValidationFailed, errors);
                return result.ToActionResult();
            }

            var loginResult = await _authService.LoginAsync(loginDto);
            return loginResult.ToActionResult();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var validationResult = await _registerValidator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var result = Result<RegisterDto>.Failure(ErrorMessages.ValidationFailed, errors);
                return result.ToActionResult();
            }

            var registerResult = await _authService.RegisterAsync(registerDto);
            return registerResult.ToCreatedResult("/api/auth/profile");
        }

        
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                var result = Result.Failure(ErrorMessages.InvalidToken);
                return result.ToUnauthorizedResult();
            }

            var logoutResult = await _authService.LogoutAsync(userId);
            return logoutResult.ToActionResult();
        }

       
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                var result = Result.Failure(ErrorMessages.InvalidToken);
                return result.ToUnauthorizedResult();
            }

            var changeResult = await _authService.ChangePasswordAsync(
                userId,
                changePasswordDto.CurrentPassword,
                changePasswordDto.NewPassword);

            return changeResult.ToActionResult();
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                var result = Result.Failure(ErrorMessages.InvalidToken);
                return result.ToUnauthorizedResult();
            }
 
            return Ok(new
            {
                success = true,
                message = "Profile endpoint - implementation needed",
                userId = userId,
                timestamp = DateTime.UtcNow
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var refreshResult = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
            return refreshResult.ToActionResult();
        }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}