using EatEat.Web.Models.Dto;
using EatEat.Web.Services.Interfaces;
using EatEat.Web.Utility;

namespace EatEat.Web.Services
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;

        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.GET,
                Url = StaticDetails.CouponAPIPath
            });
        }

        public async Task<ResponseDto?> GetCouponByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.GET,
                Url = StaticDetails.CouponAPIPath + id
            });
        }

        public async Task<ResponseDto?> GetCouponByCodeAsync(string CouponCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.GET,
                Url = StaticDetails.CouponAPIPath + "GetByCode/" + CouponCode
            });
        }

        public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.POST,
                Url = StaticDetails.CouponAPIPath,
                Data = couponDto
            });
        }

        public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.PUT,
                Url = StaticDetails.CouponAPIPath,
                Data = couponDto
            });
        }

        public async Task<ResponseDto?> DeleteCouponAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.DELETE,
                Url = StaticDetails.CouponAPIPath + id
            });
        }

    }
}