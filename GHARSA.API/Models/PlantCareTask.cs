
using System.ComponentModel.DataAnnotations.Schema;

namespace GHARSA.API.Models
{
    public class PlantCareTask
    {
        public int PlantId { get; set; }

        [ForeignKey(nameof(PlantId))]
        public Plant Plant { get; set; } = null!;

        public int CareTaskId { get; set; }

        [ForeignKey(nameof(CareTaskId))]
        public CareTask CareTask { get; set; } = null!;

        public int? IntervalDays { get; set; }
    }
}
