using System.ComponentModel.DataAnnotations;

namespace BidFlow.Entities
{
    public abstract class BaseEntity : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
