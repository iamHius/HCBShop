using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HCBShop.Data;
using HCBShop.Models;
using HCBShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HCBShop.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BillsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {   
            _context = context;
            _userManager = userManager;
        }

        // GET: Bills
        [Route("lich-su-dat-hang")]
        public async Task<IActionResult> Index()
        {
            var currentUser = _userManager.GetUserId(User);
            var applicationDbContext = _context.Bills
                .Include(b => b.StatusDelivery)
                .Where(b => b.UserId == currentUser);
            return View(await applicationDbContext.ToListAsync());  
        }

        // GET: Bills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills
                .Include(b => b.StatusDelivery)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BillId == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // GET: Bills/Create
        [Authorize (Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["StatusId"] = new SelectList(_context.StatusDeliveries, "StatusId", "StatusName");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Bills/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("BillId,UserId,StatusId,DateBooking,DateShip,UserPhone,UserEmail,Address,PaymentMethod,DeliveryMethod,Note")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StatusId"] = new SelectList(_context.StatusDeliveries, "StatusId", "StatusName", bill.StatusId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", bill.UserId);
            return View(bill);
        }

        // GET: Bills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            ViewData["StatusId"] = new SelectList(_context.StatusDeliveries, "StatusId", "StatusName", bill.StatusId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", bill.UserId);
            return View(bill);
        }

        // POST: Bills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BillId,UserId,StatusId,DateBooking,DateShip,UserPhone,UserEmail,Address,PaymentMethod,DeliveryMethod,Note")] Bill bill)
        {
            if (id != bill.BillId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.BillId))
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
            ViewData["StatusId"] = new SelectList(_context.StatusDeliveries, "StatusId", "StatusName", bill.StatusId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", bill.UserId);
            return View(bill);
        }

        // GET: Bills/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills
                .Include(b => b.StatusDelivery)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BillId == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill != null)
            {
                _context.Bills.Remove(bill);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillExists(int id)
        {
            return _context.Bills.Any(e => e.BillId == id);
        }
    }
}
