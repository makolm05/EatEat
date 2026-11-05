using EatEat.Services.EmailAPI.Messages;
using EatEat.Services.EmailAPI.Models.Dto;

namespace EatEat.Services.EmailAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task EmailCart(CartDto cartDto);
        Task RegisterUserEmail(string email);
        Task LogOrderPlaced(RewardsMessage rewardsMessage);
    }
}