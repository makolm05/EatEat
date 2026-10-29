using Microsoft.AspNetCore.Mvc;
using EatEat.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EatEat.Web.Models.Dto;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;

namespace EatEat.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await LoadCartBasedOnLoggedInUser());
        }

        public async Task<IActionResult> CartDetailsRemove(int ID)
        {
            var response = await _cartService.RemoveFromCartAsync(ID);

            if(response != null && response.IsSuccess)
            {
                TempData["success"] = "Product was removed successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["success"] = response.Message;
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

            TempData["success"] = response.Message;
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

            TempData["success"] = response.Message;
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

            TempData["success"] = response.Message;
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