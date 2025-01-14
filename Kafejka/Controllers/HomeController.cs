using System.Diagnostics;
using Kafejka.Data;
using Kafejka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kafejka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = (int)DateTime.Now.DayOfWeek;

            // mapowanie dni tygodnia 
            var dayMenuItemIds = new Dictionary<int, int>
        {
            { 1, 1 }, // Poniedzia³ek
            { 2, 3 }, // Wtorek
            { 3, 5 }, // Œroda
            { 4, 7 }, // Czwartek
            { 5, 9 }, // Pi¹tek
            { 6, 2 }, // Sobota
            { 0, 4 }  // Niedziela
        };

            var dayMenuItemId = dayMenuItemIds[today];

            var menuItem = await _context.MenuItem
                .Where(m => m.Id == dayMenuItemId)
                .FirstOrDefaultAsync();

            // Jeœli pozycja dnia nie istnieje, zwróæ widok bez tej pozycji
            if (menuItem != null)
            {
                ViewData["DayMenuItem"] = menuItem;
            }

            return View();
        }



        public IActionResult About()
        {
            return View();
        }

        public IActionResult Info()
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
