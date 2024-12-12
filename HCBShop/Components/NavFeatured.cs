using HCBShop.Data;
using Microsoft.AspNetCore.Mvc;

namespace HCBShop.Components
{
    public class NavFeatured : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public NavFeatured(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            return View("Index");
        }
    }
}
