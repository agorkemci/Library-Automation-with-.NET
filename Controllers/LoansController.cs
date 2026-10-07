using Library_Automation.Models;
using Library_Automation.ViewModels;
using LibraryAutomation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Automation.Controllers
{
    [Authorize]//Giriş yapmış olmak yeterli(Admin veya User)
    public class LoansController : Controller
    {
        private const int MaxActiveLoans = 3;
        private const int LoanDays = 14;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoansController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]//Token kontrolü olmazsa CSRF'e açıksın: kötü niyetli bir site, giriş yapmış kullanıcının tarayıcısıyla senin sitene gizlice form gönderip işlem yapabilir
        public async Task<IActionResult> Borrow(int bookId)
        {
            var userId = _userManager.GetUserId(User)!;
            var bookExist = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExist)
            {

                return NotFound();
            }
            else
            {
                var activeCount = await _context.Loans.CountAsync(l => l.UserId == userId && l.ReturnDate == null); ;
                if (activeCount >= MaxActiveLoans)
                {
                    TempData["Error"] = $"The number of maximum loanable books that an user able to loan is {MaxActiveLoans}!";
                    return RedirectToAction(actionName: "Index", controllerName: "Catalog");
                }
                //any şartı sağlayan en az bir kayıt varsa true döner
                var alreadyBorrowed = await _context.Loans.AnyAsync(l => l.BookId == bookId && l.ReturnDate == null); ;
                if (alreadyBorrowed)
                {
                    TempData["Error"] = "This book is already on loan";
                    return RedirectToAction("Index", "Catalog");

                }
                var loan = new Loan()
                {
                    BookId = bookId,
                    UserId = userId,
                    LoanDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(LoanDays)
                    //Ödünç alma tarihinden LoanDays kadar gün sonrasını son teslim tarihi olarak belirle
                };
                _context.Loans.Add(loan);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "The book is already taken!";
                    return RedirectToAction("Index", "Catalog");
                }
                TempData["Success"] = $"The book is taken. Last return date: {loan.DueDate:dd.MM.yyyy}";
                return RedirectToAction(nameof(My));
            }

        }
        [HttpGet]
        public async Task<IActionResult> My()
        {
            var userId = _userManager.GetUserId(User);
            var loans = await _context.Loans
                .AsNoTracking()
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.LoanDate)
                .Select(l => new LoanListItemViewModel
                {
                    Id = l.Id,
                    BookTitle = l.Book!.Title,
                    BookISBN = l.Book.ISBN,
                    UserFullName = l.User!.FullName,
                    UserEmail = l.User.Email!,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate
                }).ToListAsync();

            return View(loans);

        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> Active()
        {
            var loans=await _context.Loans.AsNoTracking()
                .Where(l=>l.ReturnDate==null)
                .OrderBy(l=>l.DueDate)
                .Select(l => new LoanListItemViewModel
                {
                    Id = l.Id,
                    BookTitle = l.Book!.Title,
                    BookISBN = l.Book.ISBN,
                    UserFullName = l.User!.FullName,
                    UserEmail = l.User.Email!,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate
                }).ToListAsync();

            return View(loans);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if(loan == null)
                return NotFound();
            if (loan.ReturnDate != null)
            {
                TempData["Error"] = "The book is already returned to library!";
                return RedirectToAction(nameof(Active));
            }
            loan.ReturnDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "The book has been returned.";
            return RedirectToAction(nameof(Active));
        }
    }
}
