﻿using HCBShop.Data;
using HCBShop.ViewModel;
using Microsoft.AspNetCore.Mvc;
using HCBShop.Helpers;
using HCBShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using HCBShop.Models;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using HCBShop.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace HCBShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PaypalClient _paypalClient;
        private readonly IVnPayService _vnPayservice;
        private readonly EmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, PaypalClient paypalClient, IVnPayService vnPayservice, EmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _paypalClient = paypalClient;
            _vnPayservice = vnPayservice;
            _emailSender = emailSender;
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
        public IActionResult CheckOut(CheckOutViewModel model, string payment = "COD")
        {


            if(ModelState.IsValid)
            {
                if (payment == "VNPAY")
                {
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = (decimal)Cart.Sum(p => Convert.ToDouble(p.Total)),
                        CreatedDate = DateTime.Now,
                        Description = $"{model.Note} {model.UserPhone}",
                        FullName = model.UserEmail,
                        OrderId = new Random().Next(1000, 100000)
                    };
                    return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
                }




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
        [HttpPost]
        public IActionResult SendEmailMarketing(SendEmailViewModel model)
        {
            
            var emailSubject = "Marketing";
            var emailBody =
                $@"  
                        <h2>Xin chào quý khách hàng: {model.Email},</h2>
                        <p>Chúng tôi rất phấn khích thông báo rằng chúng tôi đã cho ra mắt một sản phẩm mới tuyệt vời - Adidas Ultraboost 22!</p>
                        <p>
                            Adidas Ultraboost 22 là một [mô tả ngắn về sản phẩm]
                            Chúng tôi tự tin rằng sản phẩm này sẽ làm bạn hài lòng và đáp ứng được nhu cầu của bạn. Đặc biệt, chúng tôi đã dành một ưu đãi đặc biệt cho những khách hàng đầu tiên:
                            Giảm giá 20% trên Adidas Ultraboost 22
                            Miễn phí vận chuyển cho đơn hàng từ 30.000 VND trở lên.
                            Đừng bỏ lỡ cơ hội này! Nhấn vào nút dưới đây để xem chi tiết sản phẩm và đặt hàng ngay hôm nay.
                            </p>
                            <p>
Nếu bạn cần thêm thông tin hoặc có bất kỳ câu hỏi nào, đừng ngần ngại liên hệ với chúng tôi qua Email HBCShop@gmail.com hoặc số điện thoại +123456789. Chúng tôi luôn sẵn sàng hỗ trợ bạn.
</p>
                            <p>
Trân trọng,
</p>
                            <p>
HCBShop hân hạnh được phục vụ quý khách !
</p>
                    ";

            _emailSender.SendEmailAsync(model.Email, emailSubject, emailBody);
            return View("Subcribe");
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
            //try
            //{
            //    var response = await _paypalClient.CaptureOrder(orderID);
            //    return Ok(response);
            //}
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);
                if (response != null && response.status == "COMPLETED")
                {
                    // Lấy thông tin người dùng hiện tại
                    var currentUserId = _userManager.GetUserId(User);
                    var UserCurrent = await _userManager.GetUserAsync(User);



                    // Tạo đơn hàng
                    var bill = new Bill
                    {
                        UserId = currentUserId,
                        UserEmail = User.Identity?.Name,
                        UserPhone = UserCurrent.Phone,
                        Address = UserCurrent.Address,
                        DateBooking = DateTime.Now,
                        PaymentMethod = "PayPal",
                        DeliveryMethod = "GHTK",
                        StatusId = 1, // Status là "Đang xử lý"
                        Note = "Thanh toán qua PayPal"
                    };

                    _context.Database.BeginTransaction();
                    try
                    {
                        _context.Add(bill);
                        _context.SaveChanges();

                        // Lưu chi tiết đơn hàng
                        var billDetails = Cart.Select(item => new BillDetails
                        {
                            BillId = bill.BillId,
                            ProductId = item.Id,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Discount = 0
                        }).ToList();

                        _context.AddRange(billDetails);
                        _context.SaveChanges();

                        // Xóa giỏ hàng
                        HttpContext.Session.Set<List<CartItem>>(Setting.Cart_Key, new List<CartItem>());

                        // Commit giao dịch
                        _context.Database.CommitTransaction();

                        // Trả về trang thành công
                        return View("Success");
                    }
                    catch (Exception)
                    {
                        _context.Database.RollbackTransaction();
                        throw;
                    }
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        #endregion


        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> PaymentCallBackAsync()
        {

            var response = _vnPayservice.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            var CurrentUser = _userManager.GetUserId(User);
            var UserCurrent = await _userManager.GetUserAsync(User);
            var bill = new Bill
            {
                UserId = CurrentUser,
                UserEmail = User.Identity?.Name,
                UserPhone = UserCurrent.Phone,
                Address = UserCurrent.Address,
                DateBooking = DateTime.Now,
                PaymentMethod = "VNPay",
                DeliveryMethod = "GHTK",
                StatusId = 2,
                Note = "Thanh toán qua VNPAY"


            };
            _context.Add(bill);
            _context.SaveChanges();

            HttpContext.Session.Set<List<CartItem>>(Setting.Cart_Key, new List<CartItem>());
            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("PaymentSuccess");
        }
    }
}
