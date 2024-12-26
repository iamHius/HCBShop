using HCBShop.Data;
using HCBShop.ViewModel;
using Microsoft.AspNetCore.Mvc;
using HCBShop.Helpers;
using HCBShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using HCBShop.Models;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace HCBShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PaypalClient _paypalClient;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, PaypalClient paypalClient)
        {
            _context = context;
            _userManager = userManager;
            _paypalClient = paypalClient;
        }
        
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(Setting.Cart_Key) ?? new List<CartItem>();
        [Route("gio-hang")]
        public IActionResult Index()
        {
            return View(Cart);
        }
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var CartShop = Cart;
            var item = CartShop.SingleOrDefault(p => p.Id == id);
            if(item == null)
            {
                var product = _context.Products.SingleOrDefault(p => p.ProductId == id);
                if(product == null)
                {
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    Id = product.ProductId,
                    Name = product.ProductName,
                    Price = product.ProductPrice,
                    Image = product.ProductImage ?? string.Empty,
                    Quantity = quantity
                };
                CartShop.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }
            HttpContext.Session.Set(Setting.Cart_Key, CartShop);
            
            return RedirectToAction("Index");
        }
        public IActionResult UpdateCart(int id)
        {
            var CartShop = Cart;
            var item = CartShop.SingleOrDefault(p => p.Id == id);
            if (item != null)
            {
                CartShop.Add(item);
                HttpContext.Session.Set(Setting.Cart_Key, CartShop);
            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var CartShop = Cart;
            var item = CartShop.SingleOrDefault(p => p.Id == id);
            if(item != null)
            {
                CartShop.Remove(item);
                HttpContext.Session.Set(Setting.Cart_Key, CartShop);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CheckOut()
        {
            ViewBag.PaypalClientId = _paypalClient.ClientId; 
            if (Cart.Count == 0)
            {
                return RedirectToAction("/");
            }
            return View(Cart);
        }

        [HttpPost]
        public IActionResult CheckOut(CheckOutViewModel model)
        {
            if(ModelState.IsValid)
            {
                var CurrentUser = _userManager.GetUserId(User);
                var bill = new Bill
                {
                    UserId = CurrentUser,
                    UserEmail = model.UserEmail,
                    UserPhone = model.UserPhone ,
                    Address = model.Address ?? null,
                    DateBooking = DateTime.Now,
                    PaymentMethod = "COD",
                    DeliveryMethod = "GHTK",
                    StatusId = 1, 
                    Note = model.Note

                };
                _context.Database.BeginTransaction();
                try
                {
                    _context.Database.CommitTransaction();
                    _context.Add(bill);
                    _context.SaveChanges();

                    var billdetails = new List<BillDetails>();
                    foreach(var item in Cart)
                    {
                        billdetails.Add(new BillDetails
                        {
                            BillId = bill.BillId,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            ProductId = item.Id,
                            Discount = 0

                        });
                    }
                    _context.AddRange(billdetails);
                    _context.SaveChanges();
                    HttpContext.Session.Set<List<CartItem>>(Setting.Cart_Key, new List<CartItem>());
                    return View("Success"); 
                }
                catch
                {
                    _context.Database.RollbackTransaction();
                }
            }
            
            return View(Cart);
        }

        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }

        #region Paypal payment

        [HttpPost("/Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            
            var tongTien = Cart.Any() ? Cart.Sum(p => p.Total).ToString("F2", CultureInfo.InvariantCulture) : "0.00";

            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);
                return Ok(response);
            }
            catch(Exception ex)
            {
                var error = new {ex.GetBaseException().Message};
                return BadRequest(error);
            }

        }
        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);
                return Ok(response);
            }
            catch(Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        #endregion

    }
}
