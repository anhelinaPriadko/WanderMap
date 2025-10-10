using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WanderMap.ViewModels
{
    public class CreatePlaceViewModel
    {
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int? CategoryId { get; set; }

        public string? Address { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string? WebsiteUrl { get; set; }

        public string? ContactPhone { get; set; }

        // Це поле буде використовуватися для завантаження одного файлу.
        public IFormFile? MainPhotoFile { get; set; }

        // Це поле для завантаження декількох файлів.
        public IFormFile[]? OtherPhotoFiles { get; set; }
    }
}