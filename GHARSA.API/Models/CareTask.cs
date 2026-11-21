
using System.ComponentModel.DataAnnotations;

namespace GHARSA.API.Models
{
    public class CareTask
    {
        [Key]
        public int CareTaskId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        // Many-to-Many
        public ICollection<PlantCareTask> PlantCareTasks { get; set; }
            = new List<PlantCareTask>();
    }
}
