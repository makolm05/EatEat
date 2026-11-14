using EatEat.Services.ProductAPI.Models;
using EatEat.Services.ProductAPI.Models.Dto;

namespace EatEat.Services.ProductAPI.Services.Interfaces
{
    public interface IProductService
    {
        public Task<ProductDto> GetProductByIDAsync(int id);
        public Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        public Task<Product> CreateProductAsync(ProductDto p);
        public Task<Product> UpdateProductAsync(ProductDto p);
        public Task SaveProductImage(Product p, string? imageUrl, string localFilePath);
        public Task DeleteProductAsync(int ID);
    }
}