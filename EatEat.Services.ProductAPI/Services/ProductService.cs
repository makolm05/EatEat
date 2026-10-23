using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EatEat.Services.ProductAPI.Data;
using EatEat.Services.ProductAPI.Models;
using EatEat.Services.ProductAPI.Models.Dto;
using EatEat.Services.ProductAPI.Services.Interfaces;

namespace EatEat.Services.ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;
        public ProductService(IMapper mapper, AppDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<ProductDto> GetProductByIDAsync(int id)
        {
            // better use repository to get products, but i wanna don't waste time on it and add new services for microservices
            return _mapper.Map<ProductDto>(await _db.Products.FirstAsync(x => x.ID == id));
        }
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            return _mapper.Map<IEnumerable<ProductDto>>(await _db.Products.ToListAsync());
        }

        public async Task<Product> CreateProductAsync(ProductDto product)
        {
            var result = _db.Add(_mapper.Map<Product>(product));
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Product> UpdateProductAsync(ProductDto product)
        {
            var entity = _mapper.Map<Product>(product);

            var result = _db.Update(entity);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteProductAsync(int ID)
        {
            var entity = _db.Products.First(x => x.ID == ID);
            _db.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}