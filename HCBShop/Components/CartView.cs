using HCBShop.Helpers;
using HCBShop.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace HCBShop.Components
{
    public class CartView : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(Setting.Cart_Key) ?? new List<CartItem>();
            return View("CartPanel",new CartModel
            {
                SoLuong = cart.Sum(p => p.Quantity),
                TongTien = cart.Sum(p => p.Total)

            });
        }
    }
}
