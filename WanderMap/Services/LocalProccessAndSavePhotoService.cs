using Microsoft.AspNetCore.Hosting;
using WanderMap.Models;

namespace WanderMap.Services
{
    public interface IProccessAndSavePhotoService
    {
        Task<Photo> ProcessAndSavePhotoAsync(IFormFile file, bool isMain);
    }
    public class LocalProccessAndSavePhotoService: IProccessAndSavePhotoService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LocalProccessAndSavePhotoService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Photo> ProcessAndSavePhotoAsync(IFormFile file, bool isMain)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

            Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "images"));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var photo = new Photo
            {
                Url = $"/images/{fileName}",
                OriginalFileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                IsMain = isMain,
                CreatedAt = DateTime.UtcNow
            };

            return photo;
        }
    }
}
