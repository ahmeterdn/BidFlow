﻿namespace BidFlow.DTOs.User
{
    public class UpdateUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}