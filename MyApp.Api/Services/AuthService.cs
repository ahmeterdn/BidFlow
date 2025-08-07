using AutoMapper;
using BidFlow.Common;
using BidFlow.DTOs.Auth;
using BidFlow.DTOs.User;
using BidFlow.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BidFlow.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _userService = userService;
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await GetUserByUsernameOrEmailAsync(loginDto.Username);

                if (user == null)
                {
                    return Result<LoginResponseDto>.Failure(ErrorMessages.InvalidCredentials);
                }

                if (!user.IsActive)
                {
                    return Result<LoginResponseDto>.Failure(ErrorMessages.UserInactive);
                }

                if (!VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    return Result<LoginResponseDto>.Failure(ErrorMessages.InvalidCredentials);
                }

                var tokenResult = await GenerateTokenAsync(user.Id);
                if (!tokenResult.IsSuccess)
                {
                    return Result<LoginResponseDto>.Failure("Token generation failed");
                }

                await _userService.UpdateLastLoginAsync(user.Id);

                var userDto = _mapper.Map<UserResponseDto>(user);
                var loginResponse = new LoginResponseDto
                {
                    Token = tokenResult.Data!,
                    RefreshToken = GenerateRefreshToken(),
                    ExpiresAt = DateTime.UtcNow.AddHours(GetTokenExpiryHours()),
                    User = userDto
                };

                return Result<LoginResponseDto>.Success(loginResponse, SuccessMessages.LoginSuccessful);
            }
            catch (Exception ex)
            {
                return Result<LoginResponseDto>.Failure($"Login failed: {ex.Message}");
            }
        }

        public async Task<Result<LoginResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var usernameExists = await _userService.IsUsernameExistsAsync(registerDto.Username);
                if (usernameExists.IsSuccess && usernameExists.Data)
                {
                    return Result<LoginResponseDto>.Failure(ErrorMessages.UsernameAlreadyExists);
                }

                var emailExists = await _userService.IsEmailExistsAsync(registerDto.Email);
                if (emailExists.IsSuccess && emailExists.Data)
                {
                    return Result<LoginResponseDto>.Failure(ErrorMessages.EmailAlreadyExists);
                }

                var user = _mapper.Map<User>(registerDto);
                user.PasswordHash = HashPassword(registerDto.Password);
                user.IsActive = true;

                var createdUser = await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var tokenResult = await GenerateTokenAsync(createdUser.Id);
                if (!tokenResult.IsSuccess)
                {
                    return Result<LoginResponseDto>.Failure("Token generation failed");
                }

                var userDto = _mapper.Map<UserResponseDto>(createdUser);
                var loginResponse = new LoginResponseDto
                {
                    Token = tokenResult.Data!,
                    RefreshToken = GenerateRefreshToken(),
                    ExpiresAt = DateTime.UtcNow.AddHours(GetTokenExpiryHours()),
                    User = userDto
                };

                return Result<LoginResponseDto>.Success(loginResponse, SuccessMessages.UserCreated);
            }
            catch (Exception ex)
            {
                return Result<LoginResponseDto>.Failure($"Registration failed: {ex.Message}");
            }
        }

        public async Task<Result> LogoutAsync(int userId)
        {
            try
            {
                // Basit logout - gerçek uygulamada token'ı blacklist'e ekleyebiliriz
                // Şimdilik sadece success döndürüyoruz

                // Activity log eklenebilir
                var activityLog = new UserActivityLog
                {
                    UserId = userId,
                    Action = "Logout",
                    Description = "User logged out",
                    IpAddress = "::1", // HttpContext'ten alınabilir
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.UserActivityLogs.AddAsync(activityLog);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success(SuccessMessages.LogoutSuccessful);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Logout failed: {ex.Message}");
            }
        }

        public async Task<Result<string>> GenerateTokenAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Result<string>.Failure(ErrorMessages.UserNotFound);
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Email, user.Email),
                    new("username", user.Username),
                    new("userId", user.Id.ToString())
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(GetTokenExpiryHours()),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Result<string>.Success(tokenString);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Token generation failed: {ex.Message}");
            }
        }

        public async Task<Result<LoginResponseDto>> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // Basit refresh token implementation
                // Gerçek uygulamada refresh token'ları database'de saklayıp validate edilmeli

                // Şimdilik placeholder
                return Result<LoginResponseDto>.Failure("Refresh token functionality not implemented");
            }
            catch (Exception ex)
            {
                return Result<LoginResponseDto>.Failure($"Token refresh failed: {ex.Message}");
            }
        }

        public async Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure(ErrorMessages.UserNotFound);
                }

                if (!VerifyPassword(currentPassword, user.PasswordHash))
                {
                    return Result.Failure(ErrorMessages.InvalidCredentials);
                }

                user.PasswordHash = HashPassword(newPassword);

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success(SuccessMessages.PasswordChanged);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Password change failed: {ex.Message}");
            }
        }

        private async Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            var userByUsername = await _unitOfWork.Users.GetAsync(u => u.Username == usernameOrEmail);
            if (userByUsername != null)
                return userByUsername;

            var userByEmail = await _unitOfWork.Users.GetAsync(u => u.Email == usernameOrEmail);
            return userByEmail;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string GenerateRefreshToken()
        {
            // Basit refresh token - gerçek uygulamada daha güvenli olmalı
            return Guid.NewGuid().ToString() + DateTime.UtcNow.Ticks.ToString();
        }

        private int GetTokenExpiryHours()
        {
            return int.TryParse(_configuration["Jwt:ExpiryHours"], out var hours) ? hours : 24;
        }
    }
}
