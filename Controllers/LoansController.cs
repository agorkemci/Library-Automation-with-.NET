using Library_Automation.Models;
using LibraryAutomation.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Automation.Controllers
{
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
        public async Task<IActionResult> Borrow(int bookId) {
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
                catch (Exception ex) {
                    TempData["Error"] = "The book is already taken!";
                    return RedirectToAction("Index", "Catalog");
                }
                TempData["Success"] = $"The book is taken. Last return date: {loan.DueDate:dd.MM.yyyy}";
                return RedirectToAction(nameof(My));
            }

        }
}
