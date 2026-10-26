using EatEat.Web.Models.Dto;
using EatEat.Web.Services.Interfaces;
using EatEat.Web.Utility;

namespace EatEat.Web.Services
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;

        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetCardByUserIDAsync(string userID)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.GET,
                Url = StaticDetails.ShoppingCartAPIPath + "GetCart/" + userID,
            });
        }

        public async Task<ResponseDto?> UpsertCartAsync(CartDto dto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.POST,
                Url = StaticDetails.ShoppingCartAPIPath + "Upsert",
                Data = dto
            });
        }

        public async Task<ResponseDto?> ApplyCouponAsync(ApplyCouponDto dto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.POST,
                Url = StaticDetails.ShoppingCartAPIPath + "ApplyCoupon",
                Data = dto
            });
        }

        public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsID)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.POST,
                Url = StaticDetails.ShoppingCartAPIPath + "RemoveCartDetails",
                Data = cartDetailsID
            });
        }
    }
}
