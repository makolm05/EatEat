using EatEat.Services.ShoppingCartAPI.Models.Dto;
using EatEat.Services.ShoppingCartAPI.Services.IService;
using Newtonsoft.Json;

namespace EatEat.Services.ShoppingCartAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory factory)
        {
            _httpClientFactory = factory;
        }

        public async Task<CouponDto> GetCouponAsync(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/Coupon/GetByCode/{couponCode}");

            var apiResponse = JsonConvert.DeserializeObject<ResponseDto>(await response.Content.ReadAsStringAsync());

            if (apiResponse.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(apiResponse.Result.ToString());
            }
            else
            {
                throw new Exception(apiResponse.Message);
            }
        }
    }
}