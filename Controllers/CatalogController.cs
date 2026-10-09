using Library_Automation.ViewModels;
using LibraryAutomation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Library_Automation.ViewModels.CatalogIndexViewModel;

namespace Library_Automation.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public CatalogController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? search,int? categoryId,bool onlyAvailable=false, int page=1)
        {
            //normalde efcore çektiği her nesneyi takip eder ve değişiklikleri izler.
            // Sadece okuma yapacağımızdan AsNoTracking() ile bunu devre dışı bırakıyoruz
            var query = _context.Books.AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                var term = search.Trim();
                query=query.Where(b=>
                b.Title.Contains(term) ||
                b.Author.Contains(term) ||
                b.ISBN.Contains(term) ||
                (b.Publisher != null && b.Publisher.Contains(term)));      
            }
            if (categoryId.HasValue)
            {
                query=query.Where(b=>b.CategoryId==categoryId.Value);   
            }
            if(onlyAvailable)
            {
                query = query.Where(b => !b.Loans.Any(l=>l.ReturnDate == null));
            }
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            if (page > totalPages)
                page = 1;
            if (page < 1)
                page = 1;

            var books=await query.OrderBy(b=>b.Title)
                .ThenBy(b=>b.Id)
                .Skip((page-1)*PageSize)
                .Take(PageSize)
                .Select(b=>new ViewModels.CatalogItemViewModel
                {
                    Id=b.Id,
                    Title=b.Title,
                    Author=b.Author,
                    Publisher=b.Publisher,
                    ISBN=b.ISBN,
                    CategoryName=b.Category!.Name,
                    Floor=b.Floor,
                    Shelf=b.Shelf,
                    IsAvailable=!b.Loans.Any(l=>l.ReturnDate==null)
                }).ToListAsync();

            var model=new CatalogIndexViewModel
            {
                Search=search,
                CategoryId=categoryId,
                OnlyAvailable=onlyAvailable,
                Page=page,
                PageSize=PageSize,
                TotalCount=totalCount,
                Books=books,
                Categories= await _context.Categories.OrderBy(c=>c.Name).Select(c=>new SelectListItem(c.Name,c.Id.ToString())
                ).ToListAsync()
            };
            return View(model);
        }
       
        




    }
}

