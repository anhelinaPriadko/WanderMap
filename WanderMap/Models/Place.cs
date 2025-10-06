using System.ComponentModel.DataAnnotations.Schema;

namespace WanderMap.Models
{
    public class Place
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!; 
        public string Slug { get; set; } = null!; 
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? ContactPhone { get; set; }
        public int? MainPhotoId { get; set; } // optional FK to Photos
        public Photo? MainPhoto { get; set; }
        public ICollection<Photo> Photos { get; set; } = new List<Photo>(); 
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
