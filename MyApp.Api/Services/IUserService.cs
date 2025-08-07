using BidFlow.Common;
using BidFlow.DTOs.User;
using BidFlow.Entities;

namespace BidFlow.Services
{
    public interface IUserService : IBaseService<User, UserResponseDto, CreateUserDto, UpdateUserDto>
    {
        Task<Result<UserResponseDto>> GetByUsernameAsync(string username);
        Task<Result<UserResponseDto>> GetByEmailAsync(string email);
        Task<Result<bool>> IsUsernameExistsAsync(string username);
        Task<Result<bool>> IsEmailExistsAsync(string email);
        Task<Result> ChangePasswordAsync(int userId, string newPassword);
        Task<Result> UpdateLastLoginAsync(int userId);
        Task<Result> ActivateUserAsync(int userId);
        Task<Result> DeactivateUserAsync(int userId);
    }
}
