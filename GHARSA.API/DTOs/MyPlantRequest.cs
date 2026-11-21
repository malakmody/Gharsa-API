
using System.ComponentModel.DataAnnotations;

namespace GHARSA.API.DTOs
{
    public class MyPlantRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int PlantId { get; set; }

        [MaxLength(100)]
        public string? Nickname { get; set; }

        public DateTime? LastWateredAt { get; set; }
    }
}
