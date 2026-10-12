using AutoMapper;

namespace EatEat.Services.CouponAPI.Helpers.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.Coupon, Models.Dto.CouponDto>().ReverseMap();
        }
    }
}