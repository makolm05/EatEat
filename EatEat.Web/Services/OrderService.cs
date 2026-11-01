using EatEat.Web.Utility;
using EatEat.Web.Models.Dto;
using EatEat.Web.Services.Interfaces;

namespace EatEat.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;

        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CreateOrderAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.POST,
                Url = StaticDetails.OrderAPIPath,
                Data = cartDto
            });
        }
    }
}