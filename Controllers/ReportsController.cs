using Library_Automation.Models;
using Library_Automation.ViewModels;
using LibraryAutomation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Automation.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ReportsController:Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;
            var totalBooks=await _context.Books.CountAsync();
            var totalMembers= await _context.Users.CountAsync();
            var activeLoans=await _context.Loans.CountAsync(l=>l.ReturnDate==null);
            var overDueCount = await _context.Loans.CountAsync(l => l.DueDate < now && l.ReturnDate == null);


            var categoryCounts = await _context.Books
                .AsNoTracking()
                .GroupBy(b => b.Category!.Name)//key groupbydan geliyor =>category.name bizim keyimiz
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            var categoryStats = categoryCounts
                .Select(x => new CategoryStatViewModel
                {
                    Name = x.Name,
                    Count = x.Count,
                    Percent = totalBooks == 0 ? 0 : Math.Round(x.Count * 100.0 / totalBooks, 1)
                }).ToList();
            var recentBooks = await _context.Books
            .AsNoTracking()
            .OrderByDescending(b => b.Id)
            .Take(5)
            .Select(b => new RecentBookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                CategoryName = b.Category!.Name
            }).ToListAsync();

            var overDueLoans=await _context.Loans
                .AsNoTracking()
                .Where(l=>l.ReturnDate==null && l.DueDate<now)
                .OrderBy(l=>l.DueDate).
                Select(l=>new LoanListItemViewModel
                {
                    Id = l.Id,
                    BookTitle = l.Book!.Title,
                    BookISBN = l.Book.ISBN,
                    UserFullName = l.User!.FullName,
                    UserEmail = l.User.Email!,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate
                })
            .ToListAsync();
            var model = new ReportViewModel
            {
                TotalBooks = totalBooks,
                TotalMembers = totalMembers,
                ActiveLoans = activeLoans,
                OverdueCount = overDueCount,
                CategoryStats = categoryStats,
                RecentBooks = recentBooks,
                OverdueLoans = overDueLoans
            };


            return View(model);

        }


    }
}
