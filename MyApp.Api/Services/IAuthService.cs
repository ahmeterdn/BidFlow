using BidFlow.Common;
using BidFlow.DTOs.Auth;

namespace BidFlow.Services
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto);
        Task<Result<LoginResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<Result> LogoutAsync(int userId);
        Task<Result<string>> GenerateTokenAsync(int userId);
        Task<Result<LoginResponseDto>> RefreshTokenAsync(string refreshToken);
        Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
