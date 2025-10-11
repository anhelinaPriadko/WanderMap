using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WanderMap.Data;
using WanderMap.Models;
using WanderMap.ViewModels;
using WanderMap.Services;

namespace WanderMap.Controllers
{
    public class PlacesController : Controller
    {
        private readonly WanderMapDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IProccessAndSavePhotoService _photoService;
        public PlacesController(
            WanderMapDbContext context,
            ISlugService slugService,
            IProccessAndSavePhotoService photoService)
        {
            _context = context;
            _slugService = slugService;
            _photoService = photoService;
        }

        private async Task<bool> CheckPlaceDublication(string Title, decimal Latitude, decimal Longitude)
        {
            return await _context.Places.AnyAsync(p =>
                p.Title == Title &&
                p.Latitude == Latitude &&
                p.Longitude == Longitude
            );
        }

        private bool CheckPlaceDublicationForEdiding(string Title, decimal Latitude, decimal Longitude, int Id)
        {
            return _context.Places.Any(p =>
                p.Id != Id &&
                p.Title == Title &&
                p.Latitude == Latitude &&
                p.Longitude == Longitude
            );
        }

        // GET: Places
        public async Task<IActionResult> Index()
        {
            var wanderMapDbContext = _context.Places.Include(p => p.Category).Include(p => p.MainPhoto);
            return View(await wanderMapDbContext.ToListAsync());
        }

        // GET: Places/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await _context.Places
                .Include(p => p.Category)
                .Include(p => p.MainPhoto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (place == null)
            {
                return NotFound();
            }

            return View(place);
        }

        // GET: Places/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        //POST: Places/Create
        //To protect from overposting attacks, enable the specific properties you want to bind to.
        //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePlaceViewModel model, CancellationToken ct)
        {
            if (
                !ModelState.IsValid ||
                model.Latitude == null ||
                model.Longitude == null
                )
                return View(model);

            var isDuplicate = await CheckPlaceDublication(
                model.Title,
                model.Latitude.Value,
                model.Longitude.Value
            );

            if (isDuplicate)
            {
                ModelState.AddModelError("Title", "Place with such title already exists!");
                return View(model);
            }

            var place = new Place
            {
                Title = model.Title,
                Slug = await _slugService.GenerateUniqueSlugAsync<Place>(_context, model.Title, p => false, ct),
                Description = model.Description,
                CategoryId = model.CategoryId,
                Address = model.Address,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                WebsiteUrl = model.WebsiteUrl,
                ContactPhone = model.ContactPhone,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                Photos = new List<Photo>()
            };

            await using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                _context.Places.Add(place);
                await _context.SaveChangesAsync(ct);
                Photo? mainPhoto = null;
                var photosToAdd = new List<Photo>();

                if (model.MainPhotoFile != null)
                {
                    var processed = await _photoService.ProcessAndSavePhotoAsync(model.MainPhotoFile, true);
                    processed.PlaceId = place.Id;
                    processed.CreatedAt = DateTime.UtcNow;
                    processed.IsMain = true;
                    photosToAdd.Add(processed);
                    mainPhoto = processed;
                }

                if (model.OtherPhotoFiles != null && model.OtherPhotoFiles.Length > 0)
                {
                    foreach (var file in model.OtherPhotoFiles)
                    {
                        var processed = await _photoService.ProcessAndSavePhotoAsync(file, false);
                        processed.PlaceId = place.Id;
                        processed.CreatedAt = DateTime.UtcNow;
                        processed.IsMain = false;
                        photosToAdd.Add(processed);
                    }
                }

                if (photosToAdd.Count > 0)
                {
                    _context.Photos.AddRange(photosToAdd);
                    await _context.SaveChangesAsync(ct);
                }

                if (mainPhoto != null)
                {
                    place.MainPhotoId = mainPhoto.Id;
                    _context.Places.Update(place);
                    await _context.SaveChangesAsync(ct);
                }

                await transaction.CommitAsync(ct);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                ModelState.AddModelError("", "An error occurred while creating the place. Please try again.");
                return View(model);
            }
        }

        // GET: Places/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await _context.Places.FindAsync(id);
            if (place == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", place.CategoryId);
            ViewData["MainPhotoId"] = new SelectList(_context.Photos, "Id", "Url", place.MainPhotoId);
            return View(place);
        }

        // POST: Places/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Slug,Description,CategoryId,Address,Latitude,Longitude,WebsiteUrl,ContactPhone,MainPhotoId,CreatedAt,UpdatedAt,IsDeleted")] Place place)
        {
            if (id != place.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(place);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaceExists(place.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", place.CategoryId);
            ViewData["MainPhotoId"] = new SelectList(_context.Photos, "Id", "Url", place.MainPhotoId);
            return View(place);
        }

        // GET: Places/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await _context.Places
                .Include(p => p.Category)
                .Include(p => p.MainPhoto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (place == null)
            {
                return NotFound();
            }

            return View(place);
        }

        // POST: Places/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var place = await _context.Places.FindAsync(id);
            if (place != null)
            {
                _context.Places.Remove(place);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlaceExists(int id)
        {
            return _context.Places.Any(e => e.Id == id);
        }
    }
}
