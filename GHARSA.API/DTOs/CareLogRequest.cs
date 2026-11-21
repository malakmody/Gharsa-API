
using System.ComponentModel.DataAnnotations;

namespace GHARSA.API.DTOs
{
    public class CareLogRequest
    {
        [Range(1, int.MaxValue)]
        public int MyPlantId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TaskName { get; set; } = string.Empty;

        public DateTime CompletedAt { get; set; }
            = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
