using AutoMapper;

namespace EatEat.Services.ProductAPI.Helpers.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.Product, Models.Dto.ProductDto>().ReverseMap();
        }
    }
}