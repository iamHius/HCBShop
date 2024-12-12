using HCBShop.Data;
using Microsoft.AspNetCore.Mvc;

namespace HCBShop.Components
{
    public class TopSaleProduct : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public TopSaleProduct(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View("Index",_context.Products.Where(p => p.Sale == true).ToList());
        }
    }
}
