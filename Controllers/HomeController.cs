using Library_Automation.Models;
using Library_Automation.ViewModels;
using LibraryAutomation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Library_Automation.Controllers
{
  
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                TotalBooks=await _context.Books.CountAsync(),
                AvailableBooks=await _context.Books.CountAsync(b=>!b.Loans.Any(l=>l.ReturnDate==null)),
                TotalCategories=await _context.Categories.CountAsync(),
                TotalMembers=await _context.Users.CountAsync()
            };
            return View(model); 
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
