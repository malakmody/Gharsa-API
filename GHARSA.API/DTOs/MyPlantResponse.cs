
namespace GHARSA.API.DTOs
{
    public class MyPlantResponse
    {
        public int MyPlantId { get; set; }

        public int PlantId { get; set; }

        public string PlantName { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public string? CategoryName { get; set; }

        public string? Nickname { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime? LastWateredAt { get; set; }
    }
}
