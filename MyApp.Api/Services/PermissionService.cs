using BidFlow.Common;
using BidFlow.Entities;

namespace BidFlow.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(IUnitOfWork unitOfWork, ILogger<PermissionService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> HasPermissionAsync(int userId, string permissionName)
        {
            try
            {
                // User'ın aktif rollerini al
                var userRoles = await _unitOfWork.Repository<UserRole>()
                    .FindAsync(ur => ur.UserId == userId &&
                                   ur.IsActive &&
                                   (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow));

                if (!userRoles.Any())
                {
                    _logger.LogDebug("User {UserId} has no active roles", userId);
                    return false;
                }

                var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

                // Bu rollerin aktif permission'larını al
                var hasPermission = await _unitOfWork.Repository<RolePermission>()
                    .ExistsAsync(rp => roleIds.Contains(rp.RoleId) &&
                                      rp.IsActive &&
                                      rp.Permission.Name == permissionName &&
                                      rp.Permission.IsActive &&
                                      rp.Role.IsActive);

                _logger.LogDebug("User {UserId} permission check for {Permission}: {HasPermission}",
                    userId, permissionName, hasPermission);

                return hasPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission {Permission} for user {UserId}", permissionName, userId);
                return false;
            }
        }

        public async Task<bool> HasRoleAsync(int userId, string roleName)
        {
            try
            {
                var hasRole = await _unitOfWork.Repository<UserRole>()
                    .ExistsAsync(ur => ur.UserId == userId &&
                                      ur.IsActive &&
                                      ur.Role.Name == roleName &&
                                      ur.Role.IsActive &&
                                      (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow));

                return hasRole;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking role {Role} for user {UserId}", roleName, userId);
                return false;
            }
        }

        public async Task<Result<IEnumerable<string>>> GetUserPermissionsAsync(int userId)
        {
            try
            {
                var userRoles = await _unitOfWork.Repository<UserRole>()
                    .FindAsync(ur => ur.UserId == userId &&
                                   ur.IsActive &&
                                   (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow));

                if (!userRoles.Any())
                {
                    return Result<IEnumerable<string>>.Success(new List<string>(), "No permissions found");
                }

                var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

                var permissions = await _unitOfWork.Repository<RolePermission>()
                    .FindAsync(rp => roleIds.Contains(rp.RoleId) &&
                                   rp.IsActive &&
                                   rp.Permission.IsActive &&
                                   rp.Role.IsActive);

                var permissionNames = permissions.Select(rp => rp.Permission.Name).Distinct().ToList();

                return Result<IEnumerable<string>>.Success(permissionNames, "Permissions retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for user {UserId}", userId);
                return Result<IEnumerable<string>>.Failure($"Error retrieving permissions: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<string>>> GetUserRolesAsync(int userId)
        {
            try
            {
                var userRoles = await _unitOfWork.Repository<UserRole>()
                    .FindAsync(ur => ur.UserId == userId &&
                                   ur.IsActive &&
                                   ur.Role.IsActive &&
                                   (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow));

                var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();

                return Result<IEnumerable<string>>.Success(roleNames, "Roles retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
                return Result<IEnumerable<string>>.Failure($"Error retrieving roles: {ex.Message}");
            }
        }

        public async Task<Result> AssignRoleToUserAsync(int userId, int roleId)
        {
            try
            {
                // Kullanıcının bu role'e sahip olup olmadığını kontrol et
                var existingUserRole = await _unitOfWork.Repository<UserRole>()
                    .GetAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (existingUserRole != null)
                {
                    if (existingUserRole.IsActive)
                    {
                        return Result.Success("User already has this role");
                    }
                    else
                    {
                        // Pasif role'ü aktif yap
                        existingUserRole.IsActive = true;
                        existingUserRole.ExpiresAt = null;
                        _unitOfWork.Repository<UserRole>().Update(existingUserRole);
                    }
                }
                else
                {
                    // Yeni role assignment oluştur
                    var userRole = new UserRole
                    {
                        UserId = userId,
                        RoleId = roleId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System" // Bu kısmı current user'dan alabilirsiniz
                    };

                    await _unitOfWork.Repository<UserRole>().AddAsync(userRole);
                }

                await _unitOfWork.SaveChangesAsync();
                return Result.Success("Role assigned successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
                return Result.Failure($"Error assigning role: {ex.Message}");
            }
        }

        public async Task<Result> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            try
            {
                var userRole = await _unitOfWork.Repository<UserRole>()
                    .GetAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive);

                if (userRole == null)
                {
                    return Result.Failure("User does not have this role");
                }

                userRole.IsActive = false;
                _unitOfWork.Repository<UserRole>().Update(userRole);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Role removed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
                return Result.Failure($"Error removing role: {ex.Message}");
            }
        }

        public async Task<bool> IsEndpointAuthorizedAsync(int userId, string endpoint, string httpMethod)
        {
            try
            {
                // User'ın permission'larını al
                var userRoles = await _unitOfWork.Repository<UserRole>()
                    .FindAsync(ur => ur.UserId == userId &&
                                   ur.IsActive &&
                                   (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow));

                if (!userRoles.Any())
                    return false;

                var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

                // Endpoint pattern'e göre permission kontrol et
                var hasEndpointPermission = await _unitOfWork.Repository<RolePermission>()
                    .ExistsAsync(rp => roleIds.Contains(rp.RoleId) &&
                                      rp.IsActive &&
                                      rp.Permission.IsActive &&
                                      rp.Role.IsActive &&
                                      IsEndpointMatch(endpoint, rp.Permission.EndpointPattern));

                return hasEndpointPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking endpoint authorization for user {UserId}, endpoint {Endpoint}", userId, endpoint);
                return false;
            }
        }

        private static bool IsEndpointMatch(string requestEndpoint, string permissionPattern)
        {
            // Basit pattern matching - daha gelişmiş regex kullanabilirsiniz
            if (permissionPattern.Contains("*"))
            {
                var pattern = permissionPattern.Replace("*", ".*");
                return System.Text.RegularExpressions.Regex.IsMatch(requestEndpoint, pattern,
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            return string.Equals(requestEndpoint, permissionPattern, StringComparison.OrdinalIgnoreCase);
        }
    }
}
