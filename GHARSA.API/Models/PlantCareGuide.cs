
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GHARSA.API.Models
{
    public class PlantCareGuide
    {
        [Key]
        public int PlantCareGuideId { get; set; }

        [Required]
        public int PlantId { get; set; }

        [ForeignKey(nameof(PlantId))]
        public Plant Plant { get; set; } = null!;

        [MaxLength(200)]
        public string? SunlightRequirement { get; set; }

        [MaxLength(200)]
        public string? WaterRequirement { get; set; }

        [MaxLength(200)]
        public string? SoilType { get; set; }

        public int WateringIntervalDays { get; set; }

        public string? AdditionalTips { get; set; }
    }
}
