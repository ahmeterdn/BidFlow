using BidFlow.Api.Data;
using BidFlow.Entities;
using Microsoft.EntityFrameworkCore;

namespace BidFlow.Data.SeedData
{
    public static class RBACSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Roles Seed
            await SeedRoles(context);

            // Permissions Seed
            await SeedPermissions(context);

            // Role-Permission Assignments
            await SeedRolePermissions(context);

            // Default Admin User Role Assignment
            await SeedDefaultAdminRole(context);
        }

        private static async Task SeedRoles(ApplicationDbContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role
                    {
                        Name = "SuperAdmin",
                        Description = "Full system access - can manage everything",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Role
                    {
                        Name = "Admin",
                        Description = "Administrative access - can manage users and basic operations",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Role
                    {
                        Name = "User",
                        Description = "Standard user access - basic operations only",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Role
                    {
                        Name = "Guest",
                        Description = "Limited access - read-only operations",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    }
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedPermissions(ApplicationDbContext context)
        {
            if (!await context.Permissions.AnyAsync())
            {
                var permissions = new List<Permission>
                {
                    // Auth Permissions
                    new Permission
                    {
                        Name = "Auth.Profile.Read",
                        Description = "Can view own profile",
                        Resource = "Auth",
                        Action = "Read",
                        EndpointPattern = "/api/auth/profile",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Permission
                    {
                        Name = "Auth.Password.Change",
                        Description = "Can change own password",
                        Resource = "Auth",
                        Action = "Update",
                        EndpointPattern = "/api/auth/change-password",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },

                    // User Management Permissions (Admin)
                    new Permission
                    {
                        Name = "Users.Read",
                        Description = "Can view user list",
                        Resource = "Users",
                        Action = "Read",
                        EndpointPattern = "/api/admin/adminusers",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Permission
                    {
                        Name = "Users.Create",
                        Description = "Can create new users",
                        Resource = "Users",
                        Action = "Create",
                        EndpointPattern = "/api/admin/adminusers",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Permission
                    {
                        Name = "Users.Update",
                        Description = "Can update user information",
                        Resource = "Users",
                        Action = "Update",
                        EndpointPattern = "/api/admin/adminusers/*",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Permission
                    {
                        Name = "Users.Delete",
                        Description = "Can delete users",
                        Resource = "Users",
                        Action = "Delete",
                        EndpointPattern = "/api/admin/adminusers/*",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Permission
                    {
                        Name = "Users.Activate",
                        Description = "Can activate/deactivate users",
                        Resource = "Users",
                        Action = "Update",
                        EndpointPattern = "/api/admin/adminusers/*/activate",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },

                    // Role Management Permissions (SuperAdmin)
                    new Permission
                    {
                        Name = "Roles.Read",
                        Description = "Can view roles",
                        Resource = "Roles",
                        Action = "Read",
                        EndpointPattern = "/api/admin/roles",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Permission
                    {
                        Name = "Roles.Create",
                        Description = "Can create roles",
                        Resource = "Roles",
                        Action = "Create",
                        EndpointPattern = "/api/admin/roles",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Permission
                    {
                        Name = "Roles.Update",
                        Description = "Can update roles",
                        Resource = "Roles",
                        Action = "Update",
                        EndpointPattern = "/api/admin/roles/*",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Permission
                    {
                        Name = "Roles.Delete",
                        Description = "Can delete roles",
                        Resource = "Roles",
                        Action = "Delete",
                        EndpointPattern = "/api/admin/roles/*",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },

                    // Permission Management (SuperAdmin)
                    new Permission
                    {
                        Name = "Permissions.Read",
                        Description = "Can view permissions",
                        Resource = "Permissions",
                        Action = "Read",
                        EndpointPattern = "/api/admin/permissions",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Permission
                    {
                        Name = "Permissions.Create",
                        Description = "Can create permissions",
                        Resource = "Permissions",
                        Action = "Create",
                        EndpointPattern = "/api/admin/permissions",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    }
                };

                await context.Permissions.AddRangeAsync(permissions);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedRolePermissions(ApplicationDbContext context)
        {
            if (!await context.RolePermissions.AnyAsync())
            {
                var superAdminRole = await context.Roles.FirstAsync(r => r.Name == "SuperAdmin");
                var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
                var userRole = await context.Roles.FirstAsync(r => r.Name == "User");

                var allPermissions = await context.Permissions.ToListAsync();

                var rolePermissions = new List<RolePermission>();

                // SuperAdmin - ALL permissions
                foreach (var permission in allPermissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = superAdminRole.Id,
                        PermissionId = permission.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    });
                }

                // Admin - User management + basic auth
                var adminPermissions = allPermissions.Where(p =>
                    p.Name.StartsWith("Users.") ||
                    p.Name.StartsWith("Auth.")).ToList();

                foreach (var permission in adminPermissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = adminRole.Id,
                        PermissionId = permission.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    });
                }

                // User - Only basic auth
                var userPermissions = allPermissions.Where(p =>
                    p.Name.StartsWith("Auth.")).ToList();

                foreach (var permission in userPermissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = userRole.Id,
                        PermissionId = permission.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    });
                }

                await context.RolePermissions.AddRangeAsync(rolePermissions);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedDefaultAdminRole(ApplicationDbContext context)
        {
            // İlk kullanıcıya SuperAdmin rolü ver
            var firstUser = await context.Users.FirstOrDefaultAsync();
            if (firstUser != null)
            {
                var existingUserRole = await context.UserRoles.AnyAsync(ur => ur.UserId == firstUser.Id);
                if (!existingUserRole)
                {
                    var superAdminRole = await context.Roles.FirstAsync(r => r.Name == "SuperAdmin");

                    var userRole = new UserRole
                    {
                        UserId = firstUser.Id,
                        RoleId = superAdminRole.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    };

                    await context.UserRoles.AddAsync(userRole);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
