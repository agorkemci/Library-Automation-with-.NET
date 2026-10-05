using Library_Automation.Models;
using Library_Automation.ViewModels;
using LibraryAutomation.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Library_Automation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var books=await _context.Books.Include(b=>b.Category).OrderByDescending(b=>b.Id).ToListAsync();
            return View(books);
        }

        [HttpGet]   
        public async Task<IActionResult> Create()
        {
            var model = new BookFormViewModel
            {
                Categories = await GetCategoryItemsAsync()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create (BookFormViewModel model)
        {
            if(await _context.Books.AnyAsync(b=>b.ISBN==model.ISBN))
                ModelState.AddModelError(nameof(model.ISBN), "A book with this ISBN already exists.");
            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategoryItemsAsync();
                return View(model);
            }
            var book = new Book
            {
                Title = model.Title,
                Author = model.Author,
                Publisher = model.Publisher,
                ISBN = model.ISBN,
                Floor = model.Floor,
                Shelf = model.Shelf,
                CategoryId = model.CategoryId
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if(book==null)
                return NotFound();
            var model = new BookFormViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Publisher = book.Publisher,
                ISBN = book.ISBN,
                Floor = book.Floor,
                Shelf = book.Shelf,
                CategoryId = book.CategoryId,
                Categories = await GetCategoryItemsAsync()
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, BookFormViewModel model)
        {
            if(id!=model.Id)
                return BadRequest();
            if(await _context.Books.AnyAsync(b=>b.ISBN==model.ISBN && b.Id!=model.Id))
            {
                ModelState.AddModelError(nameof(model.ISBN), "A book with this ISBN already exists.");
            }
            if(!ModelState.IsValid)
            {
                model.Categories = await GetCategoryItemsAsync();
                return View(model);
            }
            var book=await _context.Books.FindAsync(id);
            if(book == null)
                return NotFound();
            book.Title = model.Title;
            book.Author = model.Author;
            book.Publisher = model.Publisher;
            book.ISBN = model.ISBN;
            book.Floor = model.Floor;
            book.Shelf = model.Shelf;
            book.CategoryId = model.CategoryId;
            await _context.SaveChangesAsync();
            TempData["Success"]= "Book updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id);
            if(book is null)
                return NotFound();  
            return View(book);
        }
        [HttpPost("Delete")]
        public async Task<IActionResult> DeletePost(int id){
            var book = await _context.Books.FindAsync(id);
            if (book is null)
                return NotFound();
            if(await _context.Loans.AnyAsync(l => l.BookId == id))
            {
                TempData["Error"] = "Cannot delete the book because it is currently on loan.";
                return RedirectToAction(nameof(Index));
            }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Book deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        private async Task<List<SelectListItem>> GetCategoryItemsAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c=>new SelectListItem(c.Name,c.Id.ToString()))
                .ToListAsync();
        }





    }
}
