using HCBShop.Data;
using Microsoft.AspNetCore.Mvc;

namespace HCBShop.Components
{
    public class TopTrendingProduct : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public TopTrendingProduct(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View("Index",_context.Products.Where(p => p.Trending == true).ToList());
        }
    }
}
