
namespace GHARSA.API.DTOs
{
    public class PlantCareGuideResponse
    {
        public int PlantCareGuideId { get; set; }

        public int PlantId { get; set; }

        public string PlantName { get; set; } = string.Empty;

        public string? SunlightRequirement { get; set; }

        public string? WaterRequirement { get; set; }

        public string? SoilType { get; set; }

        public int WateringIntervalDays { get; set; }

        public string? AdditionalTips { get; set; }
    }
}
