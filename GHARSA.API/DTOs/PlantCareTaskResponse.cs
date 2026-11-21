
namespace GHARSA.API.DTOs
{
    public class PlantCareTaskResponse
    {
        public int PlantId { get; set; }

        public string PlantName { get; set; } = string.Empty;

        public int CareTaskId { get; set; }

        public string CareTaskName { get; set; } = string.Empty;

        public int? IntervalDays { get; set; }
    }
}
