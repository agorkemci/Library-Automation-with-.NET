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
        private readonly IWebHostEnvironment _env;
        //wwwroot klasörünün diskteki gerçek yolunu veren bir servis
        public BooksController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
                PublishYear = book.PublishYear,
                PageCount=book.PageCount,
                CoverFileName=book.CoverFileName,
                Categories=await GetCategoryItemsAsync()
            }; 
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, BookFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories=await GetCategoryItemsAsync();
                model.CoverFileName=await _context.Books.AsNoTracking()
                    .Where(b=>b.Id==id).Select(b=>b.CoverFileName).FirstOrDefaultAsync();
                return View(model);
            }


            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            var oldCover = book.CoverFileName;
            book.Title = model.Title;
            book.Author = model.Author;
            book.Publisher = model.Publisher;
            book.ISBN = model.ISBN;
            book.Floor = model.Floor;
            book.Shelf = model.Shelf;
            book.CategoryId = model.CategoryId;
            book.PublishYear = model.PublishYear;
            book.PageCount = model.PageCount;


            string? newCover = null;
            if(model.CoverFile is { Length: > 0 })
            {
                newCover=await SaveCoverAsync(model.CoverFile);
                book.CoverFileName = newCover;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                DeleteCover(newCover);
                //wwwroot upload klasöründen yüklenmesine müsaade edilmiş dosyayı silip
                // asp validation summary üzerinde gösterilecek hatayı tanımlayıp
                //modelin pathini eski path yapıp modeli tekrar edit.cshtml için dönüyoruz
                ModelState.AddModelError(string.Empty, "An exception occured on updating record on db, the new Cover file will be ignored");
                model.Categories = await GetCategoryItemsAsync();
                model.CoverFileName = oldCover;
                return View(model);
            }
            if (newCover != null)
            {
                DeleteCover(oldCover);
            }
            TempData["Success"] = "Book Updated";
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
            DeleteCover(book.CoverFileName);//db den kaldırıldığı gibi wwwrootdan da siliyoruz uploded cover file'ı
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
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxCoverBytes = 2 * 1024 * 1024;

        private string CoversFolder=> Path.Combine(_env.WebRootPath,"uploads","covers");
        
        private void ValidateBookExtras(BookFormViewModel model)
        {
            if (model.PublishYear > DateTime.UtcNow.Year)
                ModelState.AddModelError(nameof(model.PublishYear), "Publication year can not be older than today's years");
            var file = model.CoverFile;
            if (file == null || file.Length == 0) return;
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                ModelState.AddModelError(nameof(model.CoverFile), "JPG, PNG or WEBP files are allowed.");
            else if (file.Length > MaxCoverBytes)
                ModelState.AddModelError(nameof(model.CoverFile), "The file must be 2 MB or smaller sized.");
        }
        private async Task<String> SaveCoverAsync(IFormFile file)
        {
            Directory.CreateDirectory(CoversFolder);
            var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
            //her dosya için özgün guidler üretiliyor. :N ise tiresiz şekilde üretmek için
            var fullPath=Path.Combine(CoversFolder, fileName);
            await using var stream = new FileStream(fullPath, FileMode.Create);//oluşturulan filepath ile dosya yoksa oluşturulur 
            //using vasıtasıyle stream ile yapılan iş tamamlandığında sonlandırılması için
            await file.CopyToAsync(stream); //orijinal veri stream üzerine yazılır
            //son olarak dosya ismi dönülür
            return fileName;

        }

        private void DeleteCover(string? fileName) {
            if(string.IsNullOrEmpty(fileName))   return;
            var fullPath=Path.Combine(CoversFolder,Path.GetFileName(fileName));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        
        }


    }
}
