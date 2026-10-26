using EatEat.Web.Models;
using EatEat.Web.Models.Dto;
using EatEat.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EatEat.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public HomeController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var list = new List<ProductDto>();
            var response = await _productService.GetAllProductsAsync();

            if (response != null && response.IsSuccess)
            {
                list.AddRange(JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result)));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            ProductDto? model = new();
            var response = await _productService.GetProductByIdAsync(id);

            if (response != null && response.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ProductDto dto)
        {
            var cartDto = new CartDto
            {
                CartHeader = new CartHeaderDto
                {
                    UserID = User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value
                }
            };

            var cartDetailsDto = new CartDetailsDto
            {
                Count = dto.Count,
                ProductID = dto.ID
            };

            cartDto.CartDetails = new List<CartDetailsDto>() { cartDetailsDto };
            var response = await _cartService.UpsertCartAsync(cartDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Item has been added";
                return RedirectToAction(nameof(Index));
            }
            else
                TempData["error"] = response?.Message;

            return View(dto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
