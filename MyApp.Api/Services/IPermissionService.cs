using BidFlow.Common;

namespace BidFlow.Services
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(int userId, string permissionName);
        Task<bool> HasRoleAsync(int userId, string roleName);
        Task<Result<IEnumerable<string>>> GetUserPermissionsAsync(int userId);
        Task<Result<IEnumerable<string>>> GetUserRolesAsync(int userId);
        Task<Result> AssignRoleToUserAsync(int userId, int roleId);
        Task<Result> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<bool> IsEndpointAuthorizedAsync(int userId, string endpoint, string httpMethod);
    }
}
