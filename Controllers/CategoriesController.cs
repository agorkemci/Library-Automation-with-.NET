using Library_Automation.Models;
using Library_Automation.ViewModels;
using LibraryAutomation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace Library_Automation.Controllers
{
    [Authorize(Roles ="Admin")]
    public class CategoriesController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var myViewModel = await _context.Categories
                .AsNoTracking()
                .OrderBy(C => C.Name)
                .Select(c => new CategoryListItemViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    BookCount = c.Books.Count
                }).ToListAsync();
            return View(myViewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            var name = model.Name?.Trim() ?? string.Empty;
            if (await _context.Categories.AnyAsync(c => c.Name == name))
            {
                ModelState.AddModelError(nameof(model.Name), "This category already exists");

            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            _context.Categories.Add(new Category { Name = name });
            await _context.SaveChangesAsync();
            TempData["Success"] = "Category added.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id) 
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) 
                return NotFound();
            return View(new CategoryFormViewModel { Id = id, Name = category.Name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();
            var name = model.Name?.Trim() ?? string.Empty;
            if (await _context.Categories.AnyAsync(c => c.Name == name && c.Id == model.Id))
            {
                ModelState.AddModelError(nameof(model.Name), "Another category already uses this name.");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var category=await _context.Categories.FindAsync(id);
            if(category== null)
                return NotFound();
            category.Name = name;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Category updated.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CategoryListItemViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    BookCount = c.Books.Count()


                }).FirstOrDefaultAsync();
            if(category==null)
                return NotFound();
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null) return NotFound();

            if (await _context.Books.AnyAsync(b => b.CategoryId == categoryId))
            {
                TempData["Error"] = "This category has books. Move or delete them first.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Category deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
