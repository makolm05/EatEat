using EatEat.Web.Models.Dto;

namespace EatEat.Web.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrderAsync(CartDto cartDto);
    }
}