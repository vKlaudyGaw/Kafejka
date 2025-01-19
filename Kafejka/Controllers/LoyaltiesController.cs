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

        // GET: Loyalties
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.Loyalty.Include(l => l.User);
        //    return View(await applicationDbContext.ToListAsync());
        //}


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

            // Inicjalizujemy zmienne
            int totalPoints = 0;
            int stampsUsed = 0;

            // Liczymy punkty
            foreach (var transaction in transactions)
            {
                totalPoints += transaction.Amount;  // Każda transakcja dodaje punkty równowartości jej kwoty
            }

            // Obliczamy liczbę pieczątek
            int currentStamps = (totalPoints / 100) - stampsUsed;

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
            ViewBag.FreeProductsAvailable = currentStamps / 5;  // 5 pieczątek = 1 darmowy produkt
            ViewBag.CurrentReward = currentReward;

            return View();
        }

        // GET: Loyalties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyalty = await _context.Loyalty
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loyalty == null)
            {
                return NotFound();
            }

            return View(loyalty);
        }

        // GET: Loyalties/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Loyalties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TotalPoints,NumberOfStampsUses,UserId")] Loyalty loyalty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loyalty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", loyalty.UserId);
            return View(loyalty);
        }

        // GET: Loyalties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyalty = await _context.Loyalty.FindAsync(id);
            if (loyalty == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", loyalty.UserId);
            return View(loyalty);
        }

        // POST: Loyalties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TotalPoints,NumberOfStampsUses,UserId")] Loyalty loyalty)
        {
            if (id != loyalty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loyalty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoyaltyExists(loyalty.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", loyalty.UserId);
            return View(loyalty);
        }

        // GET: Loyalties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyalty = await _context.Loyalty
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loyalty == null)
            {
                return NotFound();
            }

            return View(loyalty);
        }

        // POST: Loyalties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loyalty = await _context.Loyalty.FindAsync(id);
            if (loyalty != null)
            {
                _context.Loyalty.Remove(loyalty);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoyaltyExists(int id)
        {
            return _context.Loyalty.Any(e => e.Id == id);
        }
    }
}
