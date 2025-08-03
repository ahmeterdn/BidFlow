namespace BidFlow.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Username { get; set; } 
        public string Email { get; set; }    
        public string PasswordHash { get; set; }

        public bool IsPro { get; set; } = false;
        public bool IsAdmin { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
