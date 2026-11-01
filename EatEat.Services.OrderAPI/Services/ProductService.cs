using Newtonsoft.Json;
using EatEat.Services.OrderAPI.Models.Dto;
using EatEat.Services.OrderAPI.Services.IService;

namespace EatEat.Services.OrderAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory factory)
        {
            _httpClientFactory = factory;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/Product");

            var apiResponse = JsonConvert.DeserializeObject<ResponseDto>(await response.Content.ReadAsStringAsync());

            if (apiResponse.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(apiResponse.Result.ToString());
            }
            else
            {
                throw new Exception(apiResponse.Message);
            }
        }
    }
}