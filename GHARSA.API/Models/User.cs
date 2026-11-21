using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GHARSA.API.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
            = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }
            = string.Empty;

        [Required]
        public string PasswordHash { get; set; }
            = string.Empty;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        [Required]
        public string Role { get; set; }
            = "User";

        public ICollection<MyPlant> MyPlants { get; set; }
            = new List<MyPlant>();
    }
}