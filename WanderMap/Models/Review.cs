using Microsoft.Extensions.Logging;

namespace WanderMap.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int? PlaceId { get; set; }
        public Place? Place { get; set; }

        public int? EventId { get; set; }
        public Event? Event { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int Rating { get; set; }  // 1–5
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }

}
