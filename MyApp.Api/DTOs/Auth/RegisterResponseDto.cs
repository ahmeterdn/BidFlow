namespace BidFlow.DTOs.Auth
{
    public class RegisterResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public AuthUserDto User { get; set; } = null!;
        public string WelcomeMessage { get; set; } = "Welcome! Your account has been created successfully.";
    }
}
