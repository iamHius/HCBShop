using HCBShop.Data;
using HCBShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace HCBShop.Components
{
    public class CategoryListTrending : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public CategoryListTrending(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View("Index", _context.Categories.ToList());
        }
    }
}
