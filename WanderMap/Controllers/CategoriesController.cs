using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WanderMap.Data;
using WanderMap.Models;
using WanderMap.Services;

namespace WanderMap.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly WanderMapDbContext _context;
        private readonly ISlugService _slugService;

        public CategoriesController(WanderMapDbContext context, ISlugService slugService)
        {
            _context = context;
            _slugService = slugService;
        }

        private bool CheckNameDublication(string Name)
        {
            return _context.Categories.Any(c => c.Name == Name);
        }

        private bool CheckNameDublicationForEdiding(string Name, int Id)
        {
            return _context.Categories.Any(c => c.Name == Name && c.Id != Id);
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Category category, CancellationToken ct)
        {
            if (CheckNameDublication(category.Name))
            {
                ModelState.AddModelError("Name", "Category with such name already exists");
            }

            if (category.Name != null){
                category.Slug = await _slugService.GenerateUniqueSlugAsync<Category>(_context, category.Name, null, ct);
                ModelState.Remove(nameof(category.Slug));
                TryValidateModel(category);
            }

            if (ModelState.IsValid)
            {
                _context.Add(category);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return BadRequest("Something went wrong!");
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category, CancellationToken ct)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (CheckNameDublicationForEdiding(category.Name, category.Id))
            {
                ModelState.AddModelError("Name", "Category with such name already exists");
            }

            var existing = await _context.Categories.FindAsync(id);
            if (existing == null)
                return NotFound();

            if(category.Name != existing.Name)
                category.Slug = await _slugService
                    .GenerateUniqueSlugAsync<Category>(
                    _context, category.Name, e => e.Id == id, ct
                    );
            else
                category.Slug = existing.Slug;

            existing.Name = category.Name;
            existing.Description = category.Description;
            existing.Slug = category.Slug;
            ModelState.Remove(nameof(category.Slug));
            TryValidateModel(category);
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return BadRequest("Something went wrong!");
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            
            if (category == null)
            {
                return BadRequest("Something went wrong!");
            }

            var isLinked = await _context.Events.AnyAsync(e => e.CategoryId == id)
                || await _context.Places.AnyAsync(p => p.CategoryId == id);

            if (isLinked)
            {
                return BadRequest("You can`t delete this category, as it has linked data!");
            }

            _context.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
