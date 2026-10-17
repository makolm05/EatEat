using EatEat.Web.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using EatEat.Web.Services.Interfaces;
using static EatEat.Web.Utility.StaticDetails;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace EatEat.Web.Controllers
{
    public class AuthController : Controller
    {
        protected readonly List<SelectListItem> _roles;
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _roles = Enum.GetNames<APIRoles>().Select(x => new SelectListItem()
            {
                Text = x,
                Value = x
            }).ToList();

            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginRequestDto());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if(result == null || !result.IsSuccess)
            {
                TempData["error"] = (result?.Message != null) ? result.Message : "Login error";
                ModelState.AddModelError("CustomError", TempData["error"].ToString());
                return View(dto);
            }

            var responseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(result.Result));
            TempData["success"] = "Login successfull";

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.RoleList = _roles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto dto)
        {
            var createUserResponse = await _authService.RegisterAsync(dto);
            if (createUserResponse == null || !createUserResponse.IsSuccess)
            {
                TempData["error"] = (createUserResponse?.Message != null) ? createUserResponse.Message : "Registration error";
                ViewBag.RoleList = _roles;
                return View(dto);
            }

            var assignRoleDto = new AssignRoleDto
            {
                Email = dto.Email,
                RoleName = (string.IsNullOrWhiteSpace(dto.Role)) ? APIRoles.Customer.ToString() : dto.Role
            };

            var assigneRole = await _authService.AssignRoleAsync(assignRoleDto);
            if(assigneRole == null || !assigneRole.IsSuccess)
            {
                TempData["error"] = (createUserResponse?.Message != null) ? createUserResponse.Message : "Assign role error";
                ViewBag.RoleList = _roles;
                return View(dto);
            }

            TempData["success"] = "Registration successfull";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
