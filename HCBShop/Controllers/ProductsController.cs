using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HCBShop.Data;
using HCBShop.Models;
using HCBShop.ViewModel;
using HCBShop.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace HCBShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public int PageSize = 12;
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }



       
        public async Task<IActionResult> Search(string keywords ,int productpage = 1)
        {
            
            return View("Index", new ProductListViewModel
            {
                 Products = await _context.Products.Where(p => p.ProductName
                 .Contains(keywords)).Skip((productpage - 1) * PageSize)
                 .Take(PageSize)
                 .ToListAsync(),
                Categories = _context.Categories.ToList(),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = productpage,
                    TotalItem = _context.Products.Count()
                }
            });
        }


        [HttpGet]
        public async Task<IActionResult> ProductsByCat(int categoryId, int productpage = 1)
        {
            return View("Index", new ProductListViewModel
            {
                Products =  await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .Skip((productpage - 1) * PageSize).Take(PageSize).ToListAsync(),
                Categories = _context.Categories.ToList(),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = productpage,
                    TotalItem = _context.Products.Count()
                }

            });
        }

        // GET: Products
        [Route("san-pham")]
        public async Task<IActionResult> Index(int productpage = 1)
        {
            //var applicationDbContext = _context.Products.Include(p => p.Category);
            //return View(await applicationDbContext.ToListAsync());
            return View(new ProductListViewModel
            {
                Products = _context.Products.Skip((productpage -1 )* PageSize).Take(PageSize),
                Categories = _context.Categories.ToList(),
                PagingInfo = new PagingInfo
                {
                    ItemsPerPage = PageSize,
                    CurrentPage = productpage,
                    TotalItem = _context.Products.Count()
                }
            });
        }

        // GET: Products/Details/5
        [Route("san-pham/chi-tiet-sp/{productName}")]
        public async Task<IActionResult> Details(string productName)
        {


            //var product = await _context.Products
            //    .Include(p => p.Category)
            //    .FirstOrDefaultAsync(m => m.ProductId == id);
            var product = _context.Products.FirstOrDefault(p => p.ProductName.ToLower().Replace(" ", "-")
            .Replace("đ", "d")
            .Replace(" ", "-")
            .Replace("--", "-") == productName
            .ToLower()
            .Replace(" ", "-")
            .Replace("đ", "d")
            .Replace(" ", "-")
            .Replace("--", "-"));

            if (product == null)
            {
                return NotFound();
            }

            return View(product);

        }

        // GET: Products/Create
        [Authorize (Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductDescription,ProductPrice,ProductQuantity,ProductImage,Trending,SecurityPolicy,DeliveryPolicy,Sale,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductDescription,ProductPrice,ProductQuantity,ProductImage,Trending,SecurityPolicy,DeliveryPolicy,Sale,CategoryId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
