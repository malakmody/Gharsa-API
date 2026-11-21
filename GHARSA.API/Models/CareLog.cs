
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GHARSA.API.Models
{
    public class CareLog
    {
        [Key]
        public int CareLogId { get; set; }

        [Required]
        public int MyPlantId { get; set; }

        [ForeignKey(nameof(MyPlantId))]
        public MyPlant MyPlant { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string TaskName { get; set; } = string.Empty;

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
