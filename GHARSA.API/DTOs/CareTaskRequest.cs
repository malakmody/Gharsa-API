
using System.ComponentModel.DataAnnotations;

namespace GHARSA.API.DTOs
{
    public class CareTaskRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }
    }
}
