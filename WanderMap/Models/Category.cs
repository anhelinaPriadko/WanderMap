using Microsoft.Extensions.Logging;

namespace WanderMap.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<Place> Places { get; set; } = new List<Place>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
