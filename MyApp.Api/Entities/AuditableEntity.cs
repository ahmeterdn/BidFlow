using System.ComponentModel.DataAnnotations;

namespace BidFlow.Entities
{
    public abstract class AuditableEntity : BaseEntity , IAuditableEntity
    {
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
    }
}
