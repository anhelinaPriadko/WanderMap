namespace WanderMap.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!; 
        public string? UserName { get; set; }
        public string? AvatarUrl { get; set; } // For external auth:
        public string? ExternalProvider { get; set; } // "Google"
        public string? ExternalId { get; set; } // provider user id 
        // If using local auth:
        public string? PasswordHash { get; set; }
        public string Role { get; set; } = "User"; // "Admin"|"User"
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
        public bool IsDeleted { get; set; }
    }
}
