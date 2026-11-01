using EatEat.MessageBus;
using EatEat.Services.IdentityAPI.Exceptions;
using EatEat.Services.IdentityAPI.Models.Dto;
using EatEat.Services.IdentityAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EatEat.Services.IdentityAPI.Controllers
{
    [ApiController]
    [Route("api/Identity")]
    public class IdentityController : ControllerBase
    {
        protected ResponseDto _response;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;

        public IdentityController(IIdentityService identityService, IConfiguration configuration, IMessageBus messageBus)
        {
            _response = new();
            _messageBus = messageBus;
            _configuration = configuration;
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

            await _messageBus.PublishMessage(model.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
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
                return BadRequest(_response);
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
                    return BadRequest(_response);
                else if (ex is RoleAlreadySetException)
                    return BadRequest(_response);

                _response.Message = "Internal server error";
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
