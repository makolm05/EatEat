using AutoMapper;
using EatEat.Services.IdentityAPI.Models;
using EatEat.Services.IdentityAPI.Models.Dto;

namespace EatEat.Services.CouponAPI.Helpers.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegistrationRequestDto, ApplicationUser>()
                .ForMember(x => x.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
                .ForMember(x => x.UserName, opt => opt.MapFrom(src => src.Email))
                .ReverseMap();

            CreateMap<ApplicationUser, UserDto>().ReverseMap();
        }
    }
}