namespace WanderMap.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public int? PlaceId { get; set; }  // нове поле
        public Place? Place { get; set; }  // навігаційна властивість

        public DateTime StartAt { get; set; }
        public DateTime? EndAt { get; set; }

        public string? WebsiteUrl { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>(); // додай, для симетрії
        public DateTime CreatedAt { get; set; }
    }

}
