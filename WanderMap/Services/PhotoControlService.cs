using Microsoft.AspNetCore.Hosting;
using WanderMap.Data;

namespace WanderMap.Services
{
    public class PhotoControlService
    {
        private readonly WanderMapDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PhotoControlService(WanderMapDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task DeletePhotoAsync(int photoId)
        {
            var photo = await _context.Photos.FindAsync(photoId);
            if (photo != null)
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, photo.Url.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                _context.Photos.Remove(photo);
                await _context.SaveChangesAsync();
            }
        }

        //public async Task AddPhoto(int? placeId, int? eventId, )
        //{
        //}
    }
}
