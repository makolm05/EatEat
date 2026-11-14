using EatEat.Web.Utility;
using EatEat.Web.Models.Dto;
using EatEat.Web.Services.Interfaces;

namespace EatEat.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;

        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetAllProductsAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.GET,
                Url = StaticDetails.ProductAPIPath
            });
        }

        public async Task<ResponseDto?> GetProductByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.GET,
                Url = StaticDetails.ProductAPIPath + id
            });
        }

        public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.POST,
                Url = StaticDetails.ProductAPIPath,
                Data = productDto,
                ContentType = StaticDetails.ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.PUT,
                Url = StaticDetails.ProductAPIPath,
                Data = productDto,
                ContentType = StaticDetails.ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDto?> DeleteProductAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = StaticDetails.APIType.DELETE,
                Url = StaticDetails.ProductAPIPath + id
            });
        }

    }
}