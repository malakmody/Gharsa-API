
namespace GHARSA.API.DTOs
{
    public class PlantResponse
    {
        public int PlantId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ScientificName { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string? SunlightRequirement { get; set; }

        public string? WaterRequirement { get; set; }

        public int? WateringIntervalDays { get; set; }
    }
}
