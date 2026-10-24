using AutoMapper;
using EatEat.Services.ShoppingCartAPI.Models;
using EatEat.Services.ShoppingCartAPI.Models.Dto;

namespace EatEat.Services.ShoppingCartAPI.Helpers.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
            CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
        }
    }
}