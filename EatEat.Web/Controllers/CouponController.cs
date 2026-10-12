using Newtonsoft.Json;
using EatEat.Web.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using EatEat.Web.Services.Interfaces;

namespace EatEat.Web.Controllers
{
    public class CouponController : Controller
    {

        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> Index()
        {
            var list = new List<CouponDto>();
            var response = await _couponService.GetAllCouponsAsync();

            if(response != null && response.IsSuccess)
            {
                list.AddRange(JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result)));
            }

            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _couponService.CreateCouponAsync(model);

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Update(int ID)
        {
            var response = await _couponService.GetCouponByIdAsync(ID);
            if (response != null && response.IsSuccess)
            {
                var coupon = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
                return View(coupon);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Update(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _couponService.UpdateCouponAsync(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int ID)
        {
            var response = await _couponService.GetCouponByIdAsync(ID);
            if (response != null && response.IsSuccess)
            {
                var coupon = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
                return View(coupon);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CouponDto model)
        {
            var response = await _couponService.DeleteCouponAsync(model.CouponId);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}