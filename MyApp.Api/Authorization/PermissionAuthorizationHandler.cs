using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BidFlow.Services;

namespace BidFlow.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PermissionAuthorizationHandler> _logger;

        public PermissionAuthorizationHandler(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<PermissionAuthorizationHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // User authenticated mi?
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                _logger.LogWarning("User not authenticated for permission: {Permission}", requirement.Permission);
                context.Fail();
                return;
            }

            // UserId'yi al
            var userIdClaim = context.User.FindFirst("userId")?.Value ??
                             context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid userId claim for permission: {Permission}", requirement.Permission);
                context.Fail();
                return;
            }

            try
            {
                // Scoped service'leri kullan
                using var scope = _serviceScopeFactory.CreateScope();
                var authorizationService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

                // User'ın bu permission'a sahip olup olmadığını kontrol et
                var hasPermission = await authorizationService.HasPermissionAsync(userId, requirement.Permission);

                if (hasPermission)
                {
                    _logger.LogInformation("User {UserId} has permission: {Permission}", userId, requirement.Permission);
                    context.Succeed(requirement);
                }
                else
                {
                    _logger.LogWarning("User {UserId} does not have permission: {Permission}", userId, requirement.Permission);
                    context.Fail();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission {Permission} for user {UserId}", requirement.Permission, userId);
                context.Fail();
            }
        }
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
