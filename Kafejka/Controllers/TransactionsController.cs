﻿using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kafejka.Data;
using Kafejka.Models;
using Kafejka.Data.Migrations;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Components.Forms;

namespace Kafejka.Controllers
{
    //[Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        private DateTime dateTimeNow = DateTime.Now;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Jeśli użytkownik nie jest zalogowany, przekieruj go na stronę LoyaltyInfo
            if (!User.Identity.IsAuthenticated)
            {
                context.Result = RedirectToAction("LoyaltyInfo", "Home");
            }

            base.OnActionExecuting(context);
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transactions = new List<Transaction>();

            if (User.IsInRole("Administrator"))
            {
                transactions = await _context.Transaction
                    .Include(t => t.User)
                    .Include(t => t.TransactionItemsList)
                    .ThenInclude(ti => ti.MenuItem)
                    .ToListAsync();
            }
            else
            {
                transactions = await _context.Transaction
                    .Include(t => t.User)
                    .Include(t => t.TransactionItemsList)
                    .ThenInclude(ti => ti.MenuItem)
                    .Where(t => t.UserId == userId)
                    .ToListAsync();
            }
           
            return View(transactions);
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transaction = new Transaction();

            if (User.IsInRole("Administrator"))
            {
                transaction = await _context.Transaction
                .Include(t => t.User)
                .Include(t => t.TransactionItemsList)
                .ThenInclude(ti => ti.MenuItem)
                .ThenInclude(mi => mi.Type)
                .FirstOrDefaultAsync(t => t.Id == id);
            }
            else
            {
                transaction = await _context.Transaction
                .Include(t => t.User)
                .Include(t => t.TransactionItemsList)
                .ThenInclude(ti => ti.MenuItem)
                .ThenInclude(mi => mi.Type)
                .Where(t => t.UserId == userId)
                .FirstOrDefaultAsync(t => t.Id == id);
            }

            if (transaction == null || (transaction.UserId != userId && !User.IsInRole("Administrator")))
            {
                return NotFound();
            }

            return View(transaction);
        }


        // GET: Transactions/Create
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Administrator"))
            {
                var adminRoleId = await _context.Roles
                    .Where(r => r.Name == "Administrator")
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                var nonAdminUsers = await _context.Users
                    .Where(u => !_context.UserRoles
                        .Any(ur => ur.UserId == u.Id && ur.RoleId == adminRoleId))
                    .ToListAsync();

                ViewData["Users"] = nonAdminUsers;
            }
            else
            {
                ViewData["Users"] = null; 
            }
            ViewData["MenuItems"] = await _context.MenuItem.ToListAsync();
            return View();
        }


        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaction transaction, int[] quantities, string? selectedUserId)
        {
            // Sprawdzenie unikalności kodu
            var existingTransaction = await _context.Transaction
                .FirstOrDefaultAsync(t => t.Code == transaction.Code);

            if (User.IsInRole("Administrator"))
            {
                var adminRoleId = await _context.Roles
                    .Where(r => r.Name == "Administrator")
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                var nonAdminUsers = await _context.Users
                    .Where(u => !_context.UserRoles
                        .Any(ur => ur.UserId == u.Id && ur.RoleId == adminRoleId))
                    .ToListAsync();

                ViewData["Users"] = nonAdminUsers;
            }
            else
            {
                ViewData["Users"] = null;
            }

            if (existingTransaction != null)
            {
                ModelState.AddModelError("Code", "Kod transakcji musi być unikalny. Taki kod już istnieje. Podaj inny kod z paragonu.");
                
                ViewData["MenuItems"] = _context.MenuItem.ToList();
                return View(transaction);
            }


            //Sprawdzanie daty (czy jest pomiędzy datą otwarcia a dzisiejszym dniem)
            if (transaction.PurchaseTime == null || transaction.PurchaseTime < new DateTime(2015, 1, 1) || transaction.PurchaseTime > dateTimeNow)
            {
                ModelState.AddModelError("PurchaseTime", $"Data zakupu musi być między datą otwarcia kawiarni a dzisiejszym dniem.");
                ViewData["MenuItems"] = _context.MenuItem.ToList();
                return View(transaction);
            }


            //Sprawdzanie wpisania przynajmniej jednej ilości przedmiotów
            if (quantities == null || quantities.All(q => q <= 0))
            {
                ModelState.AddModelError("TransactionItemsList", "Musisz wybrać przynajmniej 1 pozycje z menu.");
                ViewData["MenuItems"] = _context.MenuItem.ToList();
                return View(transaction);
            }


            //Sprawdzanie kwoty czy jest większa od zera i czy nie przekracza 999999
            if (transaction.Amount < 0 || transaction.Amount > 999999 || transaction.Amount == null || !(transaction.Amount is int))
            {
                ModelState.AddModelError("Amount", "Podaj nieujemną oraz mniejszą niż 1000000 zł kwotę z paragonu. Przy wpisywaniu nie używaj przecinków i kropek.");
                ViewData["MenuItems"] = _context.MenuItem.ToList();
                return View(transaction);
            }

            try
            {
                //Dodawanie id oraz approved
                if (User.IsInRole("Administrator") && !string.IsNullOrEmpty(selectedUserId))
                {
                    transaction.UserId = selectedUserId;
                    transaction.Approved = true;
                }
                else
                {
                    transaction.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    transaction.Approved = false;
                }

                _context.Add(transaction);
                await _context.SaveChangesAsync();

                // Tworzenie listy TransactionItemsList
                var menuItems = _context.MenuItem.ToList();
                var transactionItemsList = new List<TransactionItemsList>();
                for (int i = 0; i < menuItems.Count; i++)
                {
                    if (quantities[i] > 0)
                    {
                        transactionItemsList.Add(new TransactionItemsList
                        {
                            TransactionId = transaction.Id,
                            MenuItemId = menuItems[i].Id,
                            Quantity = quantities[i],

                        });
                    }
                }
                
                // Zapisanie pozycji menu
                _context.TransactionItemsList.AddRange(transactionItemsList);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving transaction: {ex.Message}");
                ModelState.AddModelError("", "Wystąpił błąd przy dodawaniu transakcji. Spróbuj ponownie.");
            }

            ViewData["MenuItems"] = _context.MenuItem.ToList();
            return View(transaction);
        }


        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transaction = new Transaction();

            if (User.IsInRole("Administrator"))
            {
                transaction = await _context.Transaction
                    .Include(t => t.User)
                    .Include(t => t.TransactionItemsList)
                    .ThenInclude(ti => ti.MenuItem)
                    .FirstOrDefaultAsync(t => t.Id == id);
            }
            else
            {
                transaction = await _context.Transaction
                    .Include (t => t.User)
                    .Include(t => t.TransactionItemsList)
                    .ThenInclude(ti => ti.MenuItem)
                    .Where(t => t.Id == id)
                    .FirstOrDefaultAsync(t => t.Id == id);
            }

                //transaction = await _context.Transaction
                //.Include(t => t.TransactionItemsList)
                //.ThenInclude(ti => ti.MenuItem)
                //.FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null || (transaction.UserId != userId && !User.IsInRole("Administrator"))) return NotFound();

            // Pobranie wszystkich pozycji menu
            var menuItems = await _context.MenuItem.ToListAsync();
            ViewData["MenuItems"] = menuItems;

            // Przygotowanie ilości (łącznie z istniejącymi danymi transakcji)
            var quantities = menuItems.Select(mi =>
            {
                var existingItem = transaction.TransactionItemsList.FirstOrDefault(ti => ti.MenuItemId == mi.Id);
                return existingItem != null ? existingItem.Quantity : 0;
            }).ToArray();

            ViewData["Quantities"] = quantities;

            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Transaction transaction, int[] quantities)
        {
            if (id != transaction.Id) return NotFound();

            var existingTransaction = await _context.Transaction
                .Include(t => t.TransactionItemsList)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTransaction == null) return NotFound();

            // Sprawdzenie unikalności kodu
            var existingCodeTransaction = await _context.Transaction
                .FirstOrDefaultAsync(t => t.Code == transaction.Code && t.Id != transaction.Id);
            if (existingCodeTransaction != null)
            {
                ModelState.AddModelError("Code", "Kod transakcji musi być unikalny. Taki kod już istnieje.");
                ViewData["MenuItems"] = await _context.MenuItem.ToListAsync();
                ViewData["Quantities"] = quantities;
                return View(transaction);
            }

            //Sprawdzanie daty (czy jest pomiędzy datą otwarcia a dzisiejszym dniem)
            if (transaction.PurchaseTime == null || transaction.PurchaseTime < new DateTime(2015, 1, 1) || transaction.PurchaseTime > dateTimeNow)
            {
                ModelState.AddModelError("PurchaseTime", "Data zakupu musi być między datą otwarcia kawiarni a dzisiejszym dniem.");
                ViewData["MenuItems"] = await _context.MenuItem.ToListAsync();
                ViewData["Quantities"] = quantities;
                return View(transaction);
            }

            //Sprawdzanie wpisania przynajmniej jednej ilości przedmiotów
            if (quantities == null || quantities.All(q => q <= 0))
            {
                ModelState.AddModelError("TransactionItemsList", "Musisz wybrać przynajmniej 1 pozycję z menu.");
                ViewData["MenuItems"] = await _context.MenuItem.ToListAsync();
                ViewData["Quantities"] = quantities;
                return View(transaction);
            }
            //Sprawdzanie kwoty czy jest większa od zera i czy nie przekracza 999999
            if (transaction.Amount < 0 || transaction.Amount > 999999 || transaction.Amount == null || !(transaction.Amount is int))
            {
                ModelState.AddModelError("Amount", "Podaj nieujemną oraz mniejszą niż 1000000 zł kwotę z paragonu. Przy wpisywaniu nie używaj przecinków i kropek.");
                ViewData["MenuItems"] = await _context.MenuItem.ToListAsync();
                ViewData["Quantities"] = quantities;
                return View(transaction);

            }

            try
            {
                // Aktualizacja transakcji
                existingTransaction.Code = transaction.Code;
                existingTransaction.Amount = transaction.Amount;
                existingTransaction.PurchaseTime = transaction.PurchaseTime;

                // Aktualizacja pozycji w transakcji
                var menuItems = await _context.MenuItem.ToListAsync();
                _context.TransactionItemsList.RemoveRange(existingTransaction.TransactionItemsList);

                var transactionItemsList = new List<TransactionItemsList>();
                for (int i = 0; i < menuItems.Count; i++)
                {
                    if (quantities[i] > 0)
                    {
                        transactionItemsList.Add(new TransactionItemsList
                        {
                            TransactionId = transaction.Id,
                            MenuItemId = menuItems[i].Id,
                            Quantity = quantities[i]
                        });
                    }
                }

                _context.TransactionItemsList.AddRange(transactionItemsList);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Wystąpił błąd podczas edycji. Spróbuj ponownie.");
            }

            ViewData["MenuItems"] = await _context.MenuItem.ToListAsync();
            ViewData["Quantities"] = quantities;
            return View(transaction);
        }


        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
            {
                return NotFound();
            }



            var transaction = new Transaction();

            if (User.IsInRole("Administrator"))
            {
                transaction = await _context.Transaction
                .Include(t => t.User)
                .Include(t => t.TransactionItemsList)
                .ThenInclude(ti => ti.MenuItem)
                .ThenInclude(mi => mi.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            }
            else
            {
                transaction = await _context.Transaction
                .Include(t => t.User)
                .Include(t => t.TransactionItemsList)
                .ThenInclude(ti => ti.MenuItem)
                .ThenInclude(mi => mi.Type)
                .Where(t => t.UserId == userId)
                .FirstOrDefaultAsync(m => m.Id == id);
            }



            // Pobierz transakcję na podstawie ID
            //var transaction = await _context.Transaction
            //    .Include(t => t.TransactionItemsList)
            //    .ThenInclude(ti => ti.MenuItem)
            //    .ThenInclude(mi => mi.Type) // załadowanie typu produktu
            //    .FirstOrDefaultAsync(m => m.Id == id);

            if (transaction == null || (transaction.UserId != userId && !User.IsInRole("Administrator")))
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transaction = await _context.Transaction
                .Include(t => t.TransactionItemsList)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null || (transaction.UserId != userId && !User.IsInRole("Administrator"))) return NotFound();

            // Usuń powiązane pozycje z TransactionItemsList
            _context.TransactionItemsList.RemoveRange(transaction.TransactionItemsList);

            // Usuń transakcję
            _context.Transaction.Remove(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            transaction.Approved = true;
            _context.Update(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //DissApprove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DissApprove(int id)
        {
            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            transaction.Approved = false;
            _context.Update(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transaction.Any(e => e.Id == id);
        }
    }
}