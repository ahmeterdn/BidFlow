using BidFlow.Common;
using BidFlow.DTOs.Admin;
using BidFlow.DTOs.Auth;
using BidFlow.DTOs.User;
using BidFlow.Entities;

namespace BidFlow.Services
{
    public interface IUserService : IBaseService<User, AdminUserDto, CreateUserDto, UpdateUserDto>
    {
        Task<Result<AuthUserDto>> GetAuthUserByIdAsync(int id);
        Task<Result<AuthUserDto>> GetAuthUserByUsernameAsync(string username);
        Task<Result<AuthUserDto>> GetAuthUserByEmailAsync(string email);
 
        Task<Result<PublicUserDto>> GetPublicUserByIdAsync(int id);
        Task<Result<PublicUserDto>> GetPublicUserByUsernameAsync(string username);

        Task<Result<bool>> IsUsernameExistsAsync(string username);
        Task<Result<bool>> IsEmailExistsAsync(string email);
        Task<Result> ChangePasswordAsync(int userId, string newPassword);
        Task<Result> UpdateLastLoginAsync(int userId);
        Task<Result> ActivateUserAsync(int userId);
        Task<Result> DeactivateUserAsync(int userId);

        Task<Result<AdminUserDto>> CreateUserForAdminAsync(CreateUserDto createDto);
    }
}
