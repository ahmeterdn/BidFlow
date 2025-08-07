using AutoMapper;
using BidFlow.Common;
using BidFlow.DTOs.User;
using BidFlow.Entities;
using System.Linq.Expressions;
using BCrypt.Net;

namespace BidFlow.Services
{
    public class UserService : BaseService<User, UserResponseDto, CreateUserDto, UpdateUserDto>, IUserService
    {
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<Result<UserResponseDto>> GetByUsernameAsync(string username)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAsync(u => u.Username == username);

                if (user == null)
                {
                    return Result<UserResponseDto>.Failure(ErrorMessages.UserNotFound);
                }

                var userDto = _mapper.Map<UserResponseDto>(user);
                return Result<UserResponseDto>.Success(userDto, SuccessMessages.DataRetrieved);
            }
            catch (Exception ex)
            {
                return Result<UserResponseDto>.Failure($"Error retrieving user by username: {ex.Message}");
            }
        }

        public async Task<Result<UserResponseDto>> GetByEmailAsync(string email)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAsync(u => u.Email == email);

                if (user == null)
                {
                    return Result<UserResponseDto>.Failure(ErrorMessages.UserNotFound);
                }

                var userDto = _mapper.Map<UserResponseDto>(user);
                return Result<UserResponseDto>.Success(userDto, SuccessMessages.DataRetrieved);
            }
            catch (Exception ex)
            {
                return Result<UserResponseDto>.Failure($"Error retrieving user by email: {ex.Message}");
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

                // Password hash'leme işlemi - BCrypt kullanabilirsiniz
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

        // CreateAsync override - validation ekleyebiliriz
        public override async Task<Result<UserResponseDto>> CreateAsync(CreateUserDto createDto)
        {
            try
            {
                // Username ve Email kontrolü
                var usernameExists = await IsUsernameExistsAsync(createDto.Username);
                if (usernameExists.IsSuccess && usernameExists.Data)
                {
                    return Result<UserResponseDto>.Failure(ErrorMessages.UsernameAlreadyExists);
                }

                var emailExists = await IsEmailExistsAsync(createDto.Email);
                if (emailExists.IsSuccess && emailExists.Data)
                {
                    return Result<UserResponseDto>.Failure(ErrorMessages.EmailAlreadyExists);
                }

                var user = _mapper.Map<User>(createDto);
                user.PasswordHash = HashPassword(createDto.Password);

                var createdUser = await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userDto = _mapper.Map<UserResponseDto>(createdUser);
                return Result<UserResponseDto>.Success(userDto, SuccessMessages.UserCreated);
            }
            catch (Exception ex)
            {
                return Result<UserResponseDto>.Failure($"Error creating user: {ex.Message}");
            }
        }

        // BaseService abstract methods implementation
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

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

    }
}
