using EatEat.Web.Models.Dto;
using EatEat.Web.Services.Interfaces;
using EatEat.Web.Utility;

namespace EatEat.Web.Services
{
    public class AuthService : IAuthService
    {
        public readonly IBaseService _baseService;
        public AuthService(IBaseService service)
        {
            _baseService = service;
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.POST,
                Url = StaticDetails.IdentityAPIPath + "Login",
                Data = dto
            });
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto dto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.POST,
                Url = StaticDetails.IdentityAPIPath + "Register",
                Data = dto
            });
        }

        public async Task<ResponseDto?> AssignRoleAsync(AssignRoleDto dto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.POST,
                Url = StaticDetails.IdentityAPIPath + "AssignRole",
                Data = dto
            });
        }
 
    }
}