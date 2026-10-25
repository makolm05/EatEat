using EatEat.Services.ShoppingCartAPI.Models.Dto;

namespace EatEat.Services.ShoppingCartAPI.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
