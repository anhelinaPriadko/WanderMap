namespace WanderMap.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public int? PlaceId { get; set; }
        public Place? Place { get; set; }
        public int? EventId { get; set; }
        public Event? Event { get; set; }
        public string Url { get; set; } = null!; 
        public string? BlobPath { get; set; }
        public string FileName { get; set; } = null!; 
        public string ContentType { get; set; } = null!; 
        public long Size { get; set; }
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }
        public int? UploadedById { get; set; }
        public User? UploadedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
