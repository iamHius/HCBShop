using HCBShop.Data;
using Microsoft.AspNetCore.Mvc;

namespace HCBShop.Components
{
    public class ListTrendingFeatured : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public ListTrendingFeatured(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            return View("Index", _context.Products.Where(p => p.Trending == true).Take(5).ToList());
        }
    }
}
