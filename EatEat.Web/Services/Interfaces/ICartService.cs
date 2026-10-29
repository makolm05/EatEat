using EatEat.Web.Models.Dto;

namespace EatEat.Web.Services.Interfaces
{
    public interface ICartService
    {
        Task<ResponseDto?> EmailCart(CartDto dto);
        Task<ResponseDto?> GetCardByUserIDAsync(string userID);
        Task<ResponseDto?> UpsertCartAsync(CartDto dto);
        Task<ResponseDto?> ApplyCouponAsync(ApplyCouponDto dto);
        Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsID);
    }
}