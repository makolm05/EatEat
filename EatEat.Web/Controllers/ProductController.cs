using Newtonsoft.Json;
using EatEat.Web.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using EatEat.Web.Services.Interfaces;

namespace EatEat.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
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

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProductAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }

            }
            return View(model);
        }

        public async Task<IActionResult> Update(int ID)
        {
            var response = await _productService.GetProductByIdAsync(ID);
            if (response != null && response.IsSuccess)
            {
                var coupon = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(coupon);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.UpdateProductAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int ID)
        {
            var response = await _productService.GetProductByIdAsync(ID);
            if (response != null && response.IsSuccess)
            {
                var coupon = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(coupon);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ProductDto model)
        {
            var response = await _productService.DeleteProductAsync(model.ID);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(model);
        }
    }
}
