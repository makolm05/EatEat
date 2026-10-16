using EatEat.Services.IdentityAPI.Models.Dto;

namespace EatEat.Services.IdentityAPI.Services.IServices
{
    public interface IIdentityService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task<string> RegisterAsync(RegistrationRequestDto dto);
        Task<ResponseDto> AssignRoleAsync(string email, string roleName);
    }
}