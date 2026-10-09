using EatEat.Web.Models.Dto;

namespace EatEat.Web.Services.Interfaces
{
    public interface IBaseService
    {
        Task<ResponseDto?> SendAsync(RequestDto dto);
    }
}