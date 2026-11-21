
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GHARSA.API.Models
{
    public class MyPlant
    {
        [Key]
        public int MyPlantId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [Required]
        public int PlantId { get; set; }

        [ForeignKey(nameof(PlantId))]
        public Plant Plant { get; set; } = null!;

        [MaxLength(100)]
        public string? Nickname { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        public DateTime? LastWateredAt { get; set; }

        // One-to-Many
        public ICollection<CareLog> CareLogs { get; set; }
            = new List<CareLog>();
    }
}
