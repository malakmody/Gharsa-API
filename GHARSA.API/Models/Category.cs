
using System.ComponentModel.DataAnnotations;

namespace GHARSA.API.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        // One-to-Many: Category -> Plants
        public ICollection<Plant> Plants { get; set; }
            = new List<Plant>();
    }
}
