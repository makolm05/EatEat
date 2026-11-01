using AutoMapper;
using Microsoft.AspNetCore.Identity;
using EatEat.Services.IdentityAPI.Data;
using EatEat.Services.IdentityAPI.Models;
using EatEat.Services.IdentityAPI.Models.Dto;
using EatEat.Services.IdentityAPI.Exceptions;
using EatEat.Services.IdentityAPI.Services.IServices;
using EatEat.MessageBus;

namespace EatEat.Services.IdentityAPI.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IMapper _mapper;
        private readonly IJWTTokenGenerator _tokenGenetrator;
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityService(
            IMapper mapper,
            IJWTTokenGenerator tokenGenerator,
            AppDbContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _tokenGenetrator = tokenGenerator;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            // better to use Repository or UserService instead of DB in service
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == dto.UserName.ToLower());
            if (user == null)
                throw new UserNotFoundException(dto.UserName);

            bool isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isValid)
                throw new UserNotFoundException(user.Id);

            var roles = await _userManager.GetRolesAsync(user);
            return new LoginResponseDto
            {
                User = _mapper.Map<UserDto>(user),
                Token = _tokenGenetrator.GenerateToken(user, roles)
            };
        }

        public async Task<string> RegisterAsync(RegistrationRequestDto dto)
        {
            var user = _mapper.Map<ApplicationUser>(dto);

            try
            {
                var result = await _userManager.CreateAsync(user, dto.Password);
                if (result.Succeeded)
                    return string.Empty;
                else
                    return result.Errors.First().Description;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<ResponseDto> AssignRoleAsync(string email, string roleName)
        {
            var response = new ResponseDto();

            // better to use Repository or UserService instead of DB in service
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            if (user == null)
                throw new UserNotFoundException(email);

            if (await _userManager.IsInRoleAsync(user, roleName))
                throw new RoleAlreadySetException(user.Id, roleName);

            if (!(await _roleManager.RoleExistsAsync(roleName))){
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await _userManager.AddToRoleAsync(user, roleName);

            return new ResponseDto 
            {
                Message = $"Role {roleName} set for user {email}"
            };
        }
    }
}