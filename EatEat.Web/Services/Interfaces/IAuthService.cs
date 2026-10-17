using EatEat.Web.Models.Dto;

namespace EatEat.Web.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDto dto);
        Task<ResponseDto?> RegisterAsync(RegistrationRequestDto dto);
        Task<ResponseDto?> AssignRoleAsync(AssignRoleDto dto);
    }
}
