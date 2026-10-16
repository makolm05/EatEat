using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EatEat.Services.IdentityAPI.Services.IServices;
using EatEat.Services.IdentityAPI.Models.Dto;
using EatEat.Services.IdentityAPI.Exceptions;

namespace EatEat.Services.IdentityAPI.Controllers
{
    [ApiController]
    [Route("api/Identity")]
    public class IdentityController : ControllerBase
    {
        protected ResponseDto _response;
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _response = new();
            _identityService = identityService;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _identityService.RegisterAsync(model);
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }

            return Ok(_response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            try
            {
                _response.Result = await _identityService.LoginAsync(model);
                return Ok(_response);
            }
            catch (UserNotFoundException)
            {
                _response.IsSuccess = false;
                _response.Message = "User or password is incorrect";
                return NotFound(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "Internal server error";
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
        {
            try
            {
                _response.Result = await _identityService.AssignRoleAsync(model.Email, model.RoleName.ToUpper());
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                if (ex is UserNotFoundException || ex is RoleNotFoundException)
                    return NotFound(_response);
                else if (ex is RoleAlreadySetException)
                    return BadRequest(_response);

                _response.Message = "Internal server error";
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
