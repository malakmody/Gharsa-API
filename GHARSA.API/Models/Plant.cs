
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GHARSA.API.Models
{
    public class Plant
    {
        [Key]
        public int PlantId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? ScientificName { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        // One-to-One
        public PlantCareGuide? PlantCareGuide { get; set; }

        // One-to-Many
        public ICollection<MyPlant> MyPlants { get; set; }
            = new List<MyPlant>();

        // Many-to-Many through PlantCareTask
        public ICollection<PlantCareTask> PlantCareTasks { get; set; }
            = new List<PlantCareTask>();
    }
}
