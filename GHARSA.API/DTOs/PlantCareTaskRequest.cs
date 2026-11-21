
using System.ComponentModel.DataAnnotations;

namespace GHARSA.API.DTOs
{
    public class PlantCareTaskRequest
    {
        [Range(1, int.MaxValue)]
        public int PlantId { get; set; }

        [Range(1, int.MaxValue)]
        public int CareTaskId { get; set; }

        [Range(1, 365)]
        public int? IntervalDays { get; set; }
    }
}
