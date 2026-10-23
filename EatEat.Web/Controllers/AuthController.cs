using Newtonsoft.Json;
using EatEat.Web.Models.Dto;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using EatEat.Web.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication;
using static EatEat.Web.Utility.StaticDetails;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EatEat.Web.Controllers
{
    public class AuthController : Controller
    {
        protected readonly List<SelectListItem> _roles;
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _roles = Enum.GetNames<APIRoles>().Select(x => new SelectListItem()
            {
                Text = x,
                Value = x
            }).ToList();

            _authService = authService;
            _tokenProvider = tokenProvider;
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
            _tokenProvider.SetToken(responseDto.Token);

            await SignInUser(responseDto);
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
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value),
                new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.First(x => x.Type == JwtRegisteredClaimNames.Name).Value),
                new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value),
                new Claim(ClaimTypes.Name, jwt.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value)
            };
            claims.Add(new Claim(ClaimTypes.Role, jwt.Claims.First(x => x.Type == nameof(ClaimTypes.Role).ToLower()).Value));

            identity.AddClaims(claims);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}