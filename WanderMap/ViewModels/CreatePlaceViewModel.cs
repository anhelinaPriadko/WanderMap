using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WanderMap.Providers;

namespace WanderMap.ViewModels
{
    public class CreatePlaceViewModel
    {
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int? CategoryId { get; set; }

        public string? Address { get; set; }

        [ModelBinder(typeof(InvariantDecimalModelBinder))]
        public decimal? Latitude { get; set; }

        [ModelBinder(typeof(InvariantDecimalModelBinder))]
        public decimal? Longitude { get; set; }

        public string? WebsiteUrl { get; set; }

        public string? ContactPhone { get; set; }

        // Це поле буде використовуватися для завантаження одного файлу.
        public IFormFile? MainPhotoFile { get; set; }

        // Це поле для завантаження декількох файлів.
        public IFormFile[]? OtherPhotoFiles { get; set; }
    }
}