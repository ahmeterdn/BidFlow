using Microsoft.AspNetCore.Authorization;

namespace BidFlow.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public string Permission { get; }
        public string? Resource { get; }
        public string? Action { get; }

        /// Requires specific permission by name
        public RequirePermissionAttribute(string permission)
        {
            Permission = permission;
            Policy = $"RequirePermission:{permission}";
        }

        /// Requires permission by resource and action
        public RequirePermissionAttribute(string resource, string action)
        {
            Resource = resource;
            Action = action;
            Permission = $"{resource}.{action}";
            Policy = $"RequirePermission:{Permission}";
        }
    }
}
