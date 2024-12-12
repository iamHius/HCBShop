using HCBShop.Data;
using Microsoft.AspNetCore.Mvc;

namespace HCBShop.Components
{
    public class CategoryList : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public CategoryList(ApplicationDbContext context)
        {
            _context = context;
        }   
        public IViewComponentResult Invoke()
        {
            return View("Index", _context.Categories.ToList());
        }
    }
}
