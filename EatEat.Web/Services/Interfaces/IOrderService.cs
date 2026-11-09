using EatEat.Web.Models.Dto;

namespace EatEat.Web.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ResponseDto?> GetAllOrdersAsync(string? userID);
        Task<ResponseDto?> GetOrderAsync(int orderID);
        Task<ResponseDto?> UpdateOrderStatusAsync(int orderID, string status);
        Task<ResponseDto?> CreateOrderAsync(CartDto cartDto);
        Task<ResponseDto?> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto);
        Task<ResponseDto?> ValidateStripeSessionAsync(int orderHeaderID);
    }
}