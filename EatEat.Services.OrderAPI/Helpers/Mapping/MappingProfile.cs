using AutoMapper;
using EatEat.Services.OrderAPI.Models;
using EatEat.Services.OrderAPI.Models.Dto;

namespace EatEat.Services.OrderAPI.Helpers.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CartHeaderDto, OrderHeaderDto>()
                .ForMember(dest => dest.OrderTotal, u => u.MapFrom(src => src.CartTotal))
                .ForMember(dest => dest.ID, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductPrice, u => u.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.ID, opt => opt.Ignore());

            CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
            CreateMap<OrderDetails, OrderDetailsDto>().ReverseMap();
        }
    }
}