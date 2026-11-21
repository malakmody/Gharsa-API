
using System.ComponentModel.DataAnnotations;

namespace GHARSA.API.DTOs
{
    public class PlantRequest
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? ScientificName { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}
