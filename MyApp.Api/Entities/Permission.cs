using System.ComponentModel.DataAnnotations;

namespace BidFlow.Entities
{
    public class Permission : AuditableEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string Resource { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty; // "Create", "Read", "Update", "Delete"

        [Required]
        [MaxLength(200)]
        public string EndpointPattern { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; } = true;

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
