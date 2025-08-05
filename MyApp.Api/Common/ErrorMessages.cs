namespace BidFlow.Common
{
    public static class ErrorMessages
    {
        // General
        public const string ValidationFailed = "Validation failed";
        public const string UnexpectedError = "An unexpected error occurred";
        public const string NotFound = "Resource not found";
        public const string Unauthorized = "Unauthorized access";
        public const string Forbidden = "Access denied";

        // User related
        public const string UserNotFound = "User not found";
        public const string UserAlreadyExists = "User already exists";
        public const string InvalidCredentials = "Invalid username or password";
        public const string UserInactive = "User account is inactive";
        public const string EmailAlreadyExists = "Email address already exists";
        public const string UsernameAlreadyExists = "Username already exists";

        // Authentication
        public const string TokenExpired = "Token has expired";
        public const string InvalidToken = "Invalid token";
        public const string LoginRequired = "Login required";
        public const string InsufficientPermissions = "Insufficient permissions";

        // Validation
        public const string RequiredField = "This field is required";
        public const string InvalidEmail = "Please enter a valid email address";
        public const string PasswordTooShort = "Password must be at least 6 characters long";
        public const string InvalidFormat = "Invalid format";
    }

    public static class SuccessMessages
    {
        // General
        public const string OperationCompleted = "Operation completed successfully";

        // User related
        public const string UserCreated = "User created successfully";
        public const string UserUpdated = "User updated successfully";
        public const string UserDeleted = "User deleted successfully";
        public const string ProfileUpdated = "Profile updated successfully";

        // Authentication
        public const string LoginSuccessful = "Login successful";
        public const string LogoutSuccessful = "Logout successful";
        public const string PasswordChanged = "Password changed successfully";

        // Data operations
        public const string DataRetrieved = "Data retrieved successfully";
        public const string DataSaved = "Data saved successfully";
    }
}
