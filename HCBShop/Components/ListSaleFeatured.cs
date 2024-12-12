using HCBShop.Data;
using Microsoft.AspNetCore.Mvc;

namespace HCBShop.Components
{
    public class ListSaleFeatured : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public ListSaleFeatured(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View("Index",_context.Products.Where(p => p.Sale == true).Take(5).ToList());
        }
    }
}
