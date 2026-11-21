
using System.ComponentModel.DataAnnotations;

namespace GHARSA.API.DTOs
{
    public class UpdateProfileRequest
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
    }
}
