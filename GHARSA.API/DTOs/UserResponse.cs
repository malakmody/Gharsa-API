
using System;

namespace GHARSA.API.DTOs
{
    public class UserResponse
    {
        public int UserId { get; set; }

        public string FullName { get; set; }
            = string.Empty;

        public string Email { get; set; }
            = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string Role { get; set; }
            = "User";
    }
}
