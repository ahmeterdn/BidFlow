using AutoMapper;
using BidFlow.Common;
using BidFlow.DTOs.User;
using BidFlow.Entities;
using System.Linq.Expressions;
using BCrypt.Net;
using BidFlow.DTOs.Admin;
using BidFlow.DTOs.Auth;

namespace BidFlow.Services
{
    public class UserService : BaseService<User, AdminUserDto, CreateUserDto, UpdateUserDto>, IUserService
    {
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<Result<AuthUserDto>> GetAuthUserByIdAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);

                if (user == null)
                {
                    return Result<AuthUserDto>.Failure(ErrorMessages.UserNotFound);
                }

                var authUserDto = _mapper.Map<AuthUserDto>(user);
                return Result<AuthUserDto>.Success(authUserDto, SuccessMessages.DataRetrieved);
            }
            catch (Exception ex)
            {
                return Result<AuthUserDto>.Failure($"Error retrieving auth user: {ex.Message}");
            }
        }

        public async Task<Result<AuthUserDto>> GetAuthUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAsync(u => u.Username == username);

                if (user == null)
                {
                    return Result<AuthUserDto>.Failure(ErrorMessages.UserNotFound);
                }

                var authUserDto = _mapper.Map<AuthUserDto>(user);
                return Result<AuthUserDto>.Success(authUserDto, SuccessMessages.DataRetrieved);
            }
            catch (Exception ex)
            {
                return Result<AuthUserDto>.Failure($"Error retrieving auth user by username: {ex.Message}");
            }
        }

        public async Task<Result<AuthUserDto>> GetAuthUserByEmailAsync(string email)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAsync(u => u.Email == email);

                if (user == null)
                {
                    return Result<AuthUserDto>.Failure(ErrorMessages.UserNotFound);
                }

                var authUserDto = _mapper.Map<AuthUserDto>(user);
                return Result<AuthUserDto>.Success(authUserDto, SuccessMessages.DataRetrieved);
            }
            catch (Exception ex)
            {
                return Result<AuthUserDto>.Failure($"Error retrieving auth user by email: {ex.Message}");
            }
        }

        public async Task<Result<PublicUserDto>> GetPublicUserByIdAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAsync(u => u.Id == id && u.IsActive);

                if (user == null)
                {
                    return Result<PublicUserDto>.Failure(ErrorMessages.UserNotFound);
                }

                var publicUserDto = _mapper.Map<PublicUserDto>(user);
                return Result<PublicUserDto>.Success(publicUserDto, SuccessMessages.DataRetrieved);
            }
            catch (Exception ex)
            {
                return Result<PublicUserDto>.Failure($"Error retrieving public user: {ex.Message}");
            }
        }

        public async Task<Result<PublicUserDto>> GetPublicUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAsync(u => u.Username == username && u.IsActive);

                if (user == null)
                {
                    return Result<PublicUserDto>.Failure(ErrorMessages.UserNotFound);
                }

                var publicUserDto = _mapper.Map<PublicUserDto>(user);
                return Result<PublicUserDto>.Success(publicUserDto, SuccessMessages.DataRetrieved);
            }
            catch (Exception ex)
            {
                return Result<PublicUserDto>.Failure($"Error retrieving public user by username: {ex.Message}");
            }
        }

        public async Task<Result<bool>> IsUsernameExistsAsync(string username)
        {
            try
            {
                var exists = await _unitOfWork.Users.ExistsAsync(u => u.Username == username);
                return Result<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error checking username existence: {ex.Message}");
            }
        }

        public async Task<Result<bool>> IsEmailExistsAsync(string email)
        {
            try
            {
                var exists = await _unitOfWork.Users.ExistsAsync(u => u.Email == email);
                return Result<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error checking email existence: {ex.Message}");
            }
        }

        public async Task<Result> ChangePasswordAsync(int userId, string newPassword)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return Result.Failure(ErrorMessages.UserNotFound);
                }

                user.PasswordHash = HashPassword(newPassword);

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success(SuccessMessages.PasswordChanged);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error changing password: {ex.Message}");
            }
        }

        public async Task<Result> UpdateLastLoginAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return Result.Failure(ErrorMessages.UserNotFound);
                }

                user.LastLoginAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Last login updated successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error updating last login: {ex.Message}");
            }
        }

        public async Task<Result> ActivateUserAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return Result.Failure(ErrorMessages.UserNotFound);
                }

                user.IsActive = true;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("User activated successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error activating user: {ex.Message}");
            }
        }

        public async Task<Result> DeactivateUserAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return Result.Failure(ErrorMessages.UserNotFound);
                }

                user.IsActive = false;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("User deactivated successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error deactivating user: {ex.Message}");
            }
        }

        public async Task<Result<AdminUserDto>> CreateUserForAdminAsync(CreateUserDto createDto)
        {
            try
            {
                var usernameExists = await IsUsernameExistsAsync(createDto.Username);
                if (usernameExists.IsSuccess && usernameExists.Data)
                {
                    return Result<AdminUserDto>.Failure(ErrorMessages.UsernameAlreadyExists);
                }

                var emailExists = await IsEmailExistsAsync(createDto.Email);
                if (emailExists.IsSuccess && emailExists.Data)
                {
                    return Result<AdminUserDto>.Failure(ErrorMessages.EmailAlreadyExists);
                }

                var user = _mapper.Map<User>(createDto);
                user.PasswordHash = HashPassword(createDto.Password);

                var createdUser = await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var adminUserDto = _mapper.Map<AdminUserDto>(createdUser);
                return Result<AdminUserDto>.Success(adminUserDto, SuccessMessages.UserCreated);
            }
            catch (Exception ex)
            {
                return Result<AdminUserDto>.Failure($"Error creating user: {ex.Message}");
            }
        }

        public override async Task<Result<AdminUserDto>> CreateAsync(CreateUserDto createDto)
        {
            return await CreateUserForAdminAsync(createDto);
        }

        protected override Expression<Func<User, bool>>? BuildSearchPredicate(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return null;

            var lowerSearchTerm = searchTerm.ToLower();
            return u => u.Username.ToLower().Contains(lowerSearchTerm) ||
                       u.Email.ToLower().Contains(lowerSearchTerm) ||
                       (u.FirstName != null && u.FirstName.ToLower().Contains(lowerSearchTerm)) ||
                       (u.LastName != null && u.LastName.ToLower().Contains(lowerSearchTerm));
        }

        protected override Func<IQueryable<User>, IOrderedQueryable<User>>? BuildOrderBy(string sortBy, string? sortDirection)
        {
            var isDescending = sortDirection?.ToLower() == "desc";

            return sortBy.ToLower() switch
            {
                "username" => q => isDescending ? q.OrderByDescending(u => u.Username) : q.OrderBy(u => u.Username),
                "email" => q => isDescending ? q.OrderByDescending(u => u.Email) : q.OrderBy(u => u.Email),
                "firstname" => q => isDescending ? q.OrderByDescending(u => u.FirstName) : q.OrderBy(u => u.FirstName),
                "lastname" => q => isDescending ? q.OrderByDescending(u => u.LastName) : q.OrderBy(u => u.LastName),
                "createdat" => q => isDescending ? q.OrderByDescending(u => u.CreatedAt) : q.OrderBy(u => u.CreatedAt),
                _ => q => q.OrderBy(u => u.Id)
            };
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

    }
}
