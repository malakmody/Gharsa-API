
namespace GHARSA.API.DTOs
{
    public class CareLogResponse
    {
        public int CareLogId { get; set; }

        public int MyPlantId { get; set; }

        public string PlantName { get; set; } = string.Empty;

        public string TaskName { get; set; } = string.Empty;

        public DateTime CompletedAt { get; set; }

        public string? Notes { get; set; }
    }
}
