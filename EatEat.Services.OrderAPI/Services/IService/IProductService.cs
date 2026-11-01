using EatEat.Services.OrderAPI.Models.Dto;

namespace EatEat.Services.OrderAPI.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
