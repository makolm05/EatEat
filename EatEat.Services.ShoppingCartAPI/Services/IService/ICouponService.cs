using EatEat.Services.ShoppingCartAPI.Models.Dto;

namespace EatEat.Services.ShoppingCartAPI.Services.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCouponAsync(string couponCode);
    }
}