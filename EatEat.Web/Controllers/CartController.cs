using Newtonsoft.Json;
using EatEat.Web.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using EatEat.Web.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using EatEat.Web.Utility;

namespace EatEat.Web.Controllers
{
    //[Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await LoadCartBasedOnLoggedInUser());
        }

        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartBasedOnLoggedInUser());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto dto)
        {
            var cart = await LoadCartBasedOnLoggedInUser();
            cart.CartHeader.Phone = dto.CartHeader.Phone;
            cart.CartHeader.Email = dto.CartHeader.Email;
            cart.CartHeader.Name = dto.CartHeader.Name;

            var response = await _orderService.CreateOrderAsync(cart);

            if (response != null && response.IsSuccess)
            {
                var orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(response.Result.ToString());
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                
                var stripeRequest = new StripeRequestDto
                {
                    ApprovedUrl = domain + "Cart/Confirmation?orderId=" + orderHeader.ID,
                    CancelUrl = domain + "Cart/Checkout",
                    OrderHeader = orderHeader
                };

                var stripeResponse = await _orderService.CreateStripeSession(stripeRequest);
                if(stripeResponse != null && stripeResponse.IsSuccess)
                {
                    var stripeSession = JsonConvert.DeserializeObject<StripeRequestDto>(stripeResponse.Result.ToString());
                    Response.Headers.Add("Location", stripeSession.StripeSessionUrl);
                    return new StatusCodeResult(StatusCodes.Status303SeeOther);
                }


                return RedirectToAction(nameof(Checkout));
            }

            TempData["error"] = response.Message;
            return RedirectToAction(nameof(Checkout));
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            var response = await _orderService.ValidateStripeSession(orderId);

            if (response != null && response.IsSuccess)
            {
                var orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(response.Result.ToString());
                if(orderHeader.Status == StaticDetails.StatusApproved)
                {
                    return View(orderId);
                }
            }
            
            //redirect to some error page
            return View(orderId);
        }
 
        public async Task<IActionResult> CartDetailsRemove(int ID)
        {
            var response = await _cartService.RemoveFromCartAsync(ID);

            if(response != null && response.IsSuccess)
            {
                TempData["success"] = "Product was removed successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto dto)
        {
            var response = await _cartService.ApplyCouponAsync(new ApplyCouponDto
            {
                UserID = dto.CartHeader.UserID,
                CouponCode = dto.CartHeader.CouponCode
            });

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Code applied successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart()
        {
            var cart = await LoadCartBasedOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;

            var response = await _cartService.EmailCart(cart);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly.";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto dto)
        {
            var response = await _cartService.ApplyCouponAsync(new ApplyCouponDto
            {
                UserID = dto.CartHeader.UserID,
                CouponCode = dto.CartHeader.CouponCode = string.Empty
            });

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Code deleted successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        private async Task<CartDto> LoadCartBasedOnLoggedInUser()
        {
            var userID = User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            var response = await _cartService.GetCardByUserIDAsync(userID);

            if (response != null && response.IsSuccess)
                return JsonConvert.DeserializeObject<CartDto>(response.Result.ToString());

            return new CartDto();
        }
    }
}