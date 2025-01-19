using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kafejka.Data;
using Kafejka.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Kafejka.Controllers
{
    [Authorize]
    public class LoyaltiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoyaltiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Pobieramy UserId z kontekstu
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Pobieramy wszystkie zatwierdzone transakcje użytkownika
            var transactions = await _context.Transaction
                .Where(t => t.UserId == userId && t.Approved == true)  // Tylko zatwierdzone transakcje
                .Include(t => t.TransactionItemsList)
                .ThenInclude(til => til.MenuItem)  // Dołączamy pozycje z transakcji
                .ToListAsync();

            var loyalty = await _context.Loyalty
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (loyalty == null)
            {
                ViewBag.TotalPoints = 0;
                ViewBag.CurrentStamps = 0;
                ViewBag.FreeProductsAvailable = 0;  
                ViewBag.CurrentReward = null;
                ViewBag.redemstamps = 0;

                return View();
            }
            else
            {
                int totalPoints = loyalty.TotalPoints;
                int stampsUsed = 5 * loyalty.NumberOfStampsUses;


                int currentStamps = (totalPoints / 100);

                // Liczymy dostępne nagrody (na podstawie najczęściej kupowanych produktów)
                var mostBoughtProduct = transactions
                    .SelectMany(t => t.TransactionItemsList)
                    .GroupBy(til => til.MenuItemId)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault();

                MenuItem currentReward = mostBoughtProduct?.FirstOrDefault()?.MenuItem;

                // Przekazujemy dane do widoku
                ViewBag.TotalPoints = totalPoints;
                ViewBag.CurrentStamps = currentStamps;
                ViewBag.FreeProductsAvailable = currentStamps / 5;
                ViewBag.CurrentReward = currentReward;
                ViewBag.redemstamps = stampsUsed;

                return View();
            }
        }

        [HttpPost]
        public IActionResult ConfirmRedemption()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RedeemStamps()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loyalty = await _context.Loyalty.FirstOrDefaultAsync(l => l.UserId == userId);

            if (loyalty != null && loyalty.FreeProductsAvailable > 0)
            {
                loyalty.NumberOfStampsUses++;
                loyalty.TotalPoints -= 500;
                await _context.SaveChangesAsync();

                var code = new Random().Next(10000, 99999).ToString();
                ViewBag.Code = code;
                ViewBag.Timer = 15 * 60; // 15 minut w sekundach

                return View("RedemptionCode");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ConfirmUse()
        {
            return RedirectToAction(nameof(Index));
        }

        private bool LoyaltyExists(int id)
        {
            return _context.Loyalty.Any(e => e.Id == id);
        }
    }
}
