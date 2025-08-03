namespace BidFlow.Entities
{
    public class UserActivityLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PerformedByUserId { get; set; }  // Admin ID
        public Guid TargetUserId { get; set; }       // Üzerinde işlem yapılan kullanıcı
        public string Action { get; set; }           // Create, Update, Delete
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;


        public User PerformedByUser { get; set; }
        public User TargetUser { get; set; }
    }
}
