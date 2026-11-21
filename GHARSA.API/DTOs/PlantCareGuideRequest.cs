
using System.ComponentModel.DataAnnotations;

namespace GHARSA.API.DTOs
{
    public class PlantCareGuideRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int PlantId { get; set; }

        [MaxLength(200)]
        public string? SunlightRequirement { get; set; }

        [MaxLength(200)]
        public string? WaterRequirement { get; set; }

        [MaxLength(200)]
        public string? SoilType { get; set; }

        [Range(1, 365)]
        public int WateringIntervalDays { get; set; } = 1;

        public string? AdditionalTips { get; set; }
    }
}
